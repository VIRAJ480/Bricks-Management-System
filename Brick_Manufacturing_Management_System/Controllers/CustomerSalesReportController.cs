using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Brick_Manufacturing_Management_System.Models;
using Brick_Manufacturing_Management_System.DBContext;

namespace Brick_Manufacturing_Management_System.Controllers
{
	public class CustomerSalesReportController : Controller
	{
		private readonly BrickErpdbContext _db;

		public CustomerSalesReportController(BrickErpdbContext db)
		{
			_db = db;
		}

		private bool IsLoggedIn() =>
			HttpContext.Session.GetString("Username") != null;

		// GET: /CustomerSalesReport/Index
		public IActionResult Index()
		{
			if (!IsLoggedIn()) return RedirectToAction("Index", "Login");

			var vm = new CustomerSalesReportVM
			{
				FromDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1),
				ToDate = DateTime.Today
			};
			return View(vm);
		}

		// POST: /CustomerSalesReport/Index
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Index(CustomerSalesReportVM vm)
		{
			if (!IsLoggedIn()) return RedirectToAction("Index", "Login");

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

				// Brick type id → name lookup
				var brickTypes = _db.BrickTypes
					.ToDictionary(b => b.BrickTypeId.ToString(), b => b.BrickTypeName);

				var sales = _db.BrickSales
					.Include(s => s.Customer)
					.Where(s => s.SalesDate >= fromD && s.SalesDate <= toD)
					.OrderBy(s => s.SalesDate)
					.ThenBy(s => s.SalesId)
					.ToList();

				var rows = sales.Select(s => new CustomerSalesReportRow
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
