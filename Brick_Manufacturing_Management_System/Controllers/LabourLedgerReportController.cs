using Brick_Manufacturing_Management_System.DBContext;
using Brick_Manufacturing_Management_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Brick_Manufacturing_Management_System.Controllers
{
	public class LabourLedgerReportController : Controller
	{
		private readonly BrickErpdbContext _ctx;

		public LabourLedgerReportController(BrickErpdbContext context)
		{
			_ctx = context;
		}

		private bool IsLoggedIn() =>
			HttpContext.Session.GetString("Username") != null;

		// ── Helper: populate labour dropdown ──────────────────────────────
		private async Task PopulateDropdowns(LabourLedgerReportVM vm)
		{
			vm.LabourOptions = await _ctx.LabourMasters
				.OrderBy(l => l.LabourName)
				.Select(l => new SelectListItem
				{
					Value = l.LabourId.ToString(),
					Text = l.LabourName
				})
				.ToListAsync();
		}

		// ══ GET — show blank filter form ──────────────────────────────────
		[HttpGet]
		public async Task<IActionResult> Index()
		{
			if (!IsLoggedIn()) return RedirectToAction("Index", "Login");

			var vm = new LabourLedgerReportVM();
			await PopulateDropdowns(vm);
			return View(vm);
		}

		// ══ POST — validate, run SP, return result ────────────────────────
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Index(LabourLedgerReportVM model)
		{
			if (!IsLoggedIn()) return RedirectToAction("Index", "Login");

			// Always rebuild dropdown before returning view
			await PopulateDropdowns(model);
			model.ReportRequested = true;

			// ── Manual validation ────────────────────────────────────────
			if (model.LabourId == null || model.LabourId == 0)
			{
				ModelState.AddModelError("LabourId", "Please select a labour.");
				return View(model);
			}
			if (model.FromDate == null)
			{
				ModelState.AddModelError("FromDate", "From Date is required.");
				return View(model);
			}
			if (model.ToDate == null)
			{
				ModelState.AddModelError("ToDate", "To Date is required.");
				return View(model);
			}
			if (model.FromDate.Value.Date > model.ToDate.Value.Date)
			{
				ModelState.AddModelError("ToDate", "To Date must be on or after From Date.");
				return View(model);
			}

			// ── Execute stored procedure ─────────────────────────────────
			var pLabourId = new SqlParameter("@LabourId", model.LabourId.Value);
			var pFromDate = new SqlParameter("@FromDate", model.FromDate.Value.Date);
			var pToDate = new SqlParameter("@ToDate", model.ToDate.Value.Date);

			var results = await _ctx.LabourLedgerResults
				.FromSqlRaw(
					"EXEC sp_GetLabourLedger @LabourId, @FromDate, @ToDate",
					pLabourId, pFromDate, pToDate)
				.ToListAsync();

			model.Result = results.FirstOrDefault();
			return View(model);
		}
	}
}