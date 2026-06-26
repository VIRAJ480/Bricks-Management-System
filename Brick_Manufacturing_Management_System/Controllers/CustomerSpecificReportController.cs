using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Brick_Manufacturing_Management_System.Models;
using Brick_Manufacturing_Management_System.DBContext;

namespace Brick_Manufacturing_Management_System.Controllers
{
	public class CustomerSpecificReportController : Controller
	{
		private readonly BrickErpdbContext _db;

		public CustomerSpecificReportController(BrickErpdbContext db)
		{
			_db = db;
		}

		private bool IsLoggedIn() =>
			HttpContext.Session.GetString("Username") != null;

		private List<SelectListItem> GetCustomerOptions() =>
			_db.CustomerMasters
				.OrderBy(c => c.CustomerName)
				.Select(c => new SelectListItem
				{
					Value = c.CustomerId.ToString(),
					Text = c.CustomerName
				})
				.ToList();

		// GET: /CustomerSpecificReport/Index
		public IActionResult Index()
		{
			if (!IsLoggedIn()) return RedirectToAction("Index", "Login");

			var vm = new CustomerSpecificReportVM
			{
				FromDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1),
				ToDate = DateTime.Today,
				CustomerOptions = GetCustomerOptions()
			};
			return View(vm);
		}

		// POST: /CustomerSpecificReport/Index
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Index(CustomerSpecificReportVM vm)
		{
			if (!IsLoggedIn()) return RedirectToAction("Index", "Login");

			vm.CustomerOptions = GetCustomerOptions();

			if (vm.CustomerId == null || vm.CustomerId == 0)
			{
				TempData["Error"] = "Please select a customer.";
				return View(vm);
			}

			if (vm.FromDate == null || vm.ToDate == null)
			{
				TempData["Error"] = "Please select both From Date and To Date.";
				return View(vm);
			}

			if (vm.FromDate > vm.ToDate)
			{
				TempData["Error"] = "From Date cannot be greater than To Date.";
				return View(vm);
			}

			try
			{
				DateOnly fromD = DateOnly.FromDateTime(vm.FromDate.Value.Date);
				DateOnly toD = DateOnly.FromDateTime(vm.ToDate.Value.Date);

				// Selected customer name (shown in report / print)
				vm.CustomerName = _db.CustomerMasters
					.Where(c => c.CustomerId == vm.CustomerId)
					.Select(c => c.CustomerName)
					.FirstOrDefault() ?? "—";

				// Brick type id → name lookup
				var brickTypes = _db.BrickTypes
					.ToDictionary(b => b.BrickTypeId.ToString(), b => b.BrickTypeName);

				var sales = _db.BrickSales
					.Include(s => s.Customer)
					.Where(s => s.CustomerId == vm.CustomerId
								&& s.SalesDate >= fromD && s.SalesDate <= toD)
					.OrderBy(s => s.SalesDate)
					.ThenBy(s => s.SalesId)
					.ToList();

				var rows = sales.Select(s => new CustomerSpecificReportRow
				{
					SalesDate     = s.SalesDate.HasValue
						? s.SalesDate.Value.ToDateTime(TimeOnly.MinValue)
						: DateTime.Today,
					CustomerName  = s.Customer != null ? (s.Customer.CustomerName ?? "—") : "—",
					BrickTypeName = (s.BrickType != null && brickTypes.ContainsKey(s.BrickType))
						? brickTypes[s.BrickType]
						: (s.BrickType ?? "—"),
					Quantity      = s.Quantity ?? 0,
					Rate          = s.Rate ?? 0,
					TotalAmount   = s.TotalAmount ?? 0
				}).ToList();

				decimal gross = rows.Sum(r => r.TotalAmount);
				foreach (var r in rows) r.Gross = gross;

				vm.ReportData = rows;
				vm.GrossTotal = gross;
			}
			catch (Exception ex)
			{
				TempData["Error"] = "Error fetching report: " + ex.Message;
			}

			return View(vm);
		}
	}
}
