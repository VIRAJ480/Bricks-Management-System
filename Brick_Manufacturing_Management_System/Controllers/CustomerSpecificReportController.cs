using Brick_Manufacturing_Management_System.DBContext;
using Brick_Manufacturing_Management_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Brick_Manufacturing_Management_System.Controllers
{
	public class CustomerSpecificReportController : Controller
	{
		private readonly BrickErpdbContext _ctx;

		public CustomerSpecificReportController(BrickErpdbContext context)
		{
			_ctx = context;
		}

		private bool IsLoggedIn() =>
			HttpContext.Session.GetString("Username") != null;

		private async Task<List<SelectListItem>> GetCustomerOptionsAsync()
		{
			return await _ctx.CustomerMasters
				.OrderBy(c => c.CustomerName)
				.Select(c => new SelectListItem
				{
					Value = c.CustomerId.ToString(),
					Text = c.CustomerName
				})
				.ToListAsync();
		}

		[HttpGet]
		public async Task<IActionResult> Index()
		{
			if (!IsLoggedIn()) return RedirectToAction("Index", "Login");

			var vm = new CustomerSpecificReportVM
			{
				CustomerOptions = await GetCustomerOptionsAsync()
			};
			return View(vm);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Index(CustomerSpecificReportVM form)
		{
			if (!IsLoggedIn()) return RedirectToAction("Index", "Login");

			var vm = new CustomerSpecificReportVM
			{
				CustomerOptions = await GetCustomerOptionsAsync(),
				CustomerId = form.CustomerId,
				FromDate = form.FromDate,
				ToDate = form.ToDate
			};

			// ── Validation ───────────────────────────────────────────────
			if (form.CustomerId == null)
			{
				TempData["Error"] = "Please select a customer.";
				return View(vm);
			}
			if (form.FromDate == null || form.ToDate == null)
			{
				TempData["Error"] = "Please select both From Date and To Date.";
				return View(vm);
			}
			if (form.FromDate > form.ToDate)
			{
				TempData["Error"] = "From Date cannot be later than To Date.";
				return View(vm);
			}

			// Convert DateTime → DateOnly for comparison with SalesDate column
			var fromDate = DateOnly.FromDateTime(form.FromDate.Value);
			var toDate = DateOnly.FromDateTime(form.ToDate.Value);

			// ── 1. Fetch brick sales (raw — all fields nullable) ──────────
			var rawSales = await _ctx.BrickSales
				.Where(s => s.CustomerId == form.CustomerId
						 && s.SalesDate != null
						 && s.SalesDate >= fromDate
						 && s.SalesDate <= toDate)
				.Join(_ctx.CustomerMasters,
					  s => s.CustomerId,
					  c => c.CustomerId,
					  (s, c) => new
					  {
						  SalesDate = s.SalesDate,          // DateOnly?
						  CustomerName = c.CustomerName,
						  BrickTypeName = s.BrickType ?? string.Empty,
						  Quantity = (decimal)(s.Quantity ?? 0),
						  Rate = s.Rate ?? 0m,
						  TotalAmount = s.TotalAmount ?? 0m
					  })
				.OrderBy(r => r.SalesDate)
				.ToListAsync();

			// ── Map to VM rows in memory (safe .Value.ToDateTime here) ────
			vm.ReportData = rawSales.Select(s => new CustomerSpecificReportRow
			{
				SalesDate = s.SalesDate!.Value.ToDateTime(TimeOnly.MinValue),
				CustomerName = s.CustomerName,
				BrickTypeName = s.BrickTypeName,
				Quantity = s.Quantity,
				Rate = s.Rate,
				TotalAmount = s.TotalAmount,
				Gross = s.TotalAmount
			}).ToList();

			vm.GrossTotal = vm.ReportData.Sum(r => r.TotalAmount);

			vm.CustomerName = vm.ReportData.FirstOrDefault()?.CustomerName
						   ?? await _ctx.CustomerMasters
								   .Where(c => c.CustomerId == form.CustomerId)
								   .Select(c => c.CustomerName)
								   .FirstOrDefaultAsync();

			// ── 2. Fetch Paid & Remaining from PendingPayment stored proc ──
			var allPending = await _ctx.Database
				.SqlQueryRaw<PendingPaymentsVM>("EXEC sp_GetPendingCustomerPayments")
				.ToListAsync();

			var customerPending = allPending
				.FirstOrDefault(p => p.CustomerId == form.CustomerId);

			vm.TotalPaid = customerPending?.TotalPaid ?? 0;
			vm.RemainingAmount = customerPending?.RemainingAmount ?? 0;

			TempData["Success"] = $"Report generated — {vm.ReportData.Count} record(s) found.";
			return View(vm);
		}
	}
}
