using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Brick_Manufacturing_Management_System.Models;
using Brick_Manufacturing_Management_System.DBContext;

namespace Brick_Manufacturing_Management_System.Controllers
{
	public class MaterialReportController : Controller
	{
		private readonly BrickErpdbContext _db;

		public MaterialReportController(BrickErpdbContext db)
		{
			_db = db;
		}

		// GET: /MaterialReport/Index
		public IActionResult Index()
		{
			var vm = new MaterialReportVM
			{
				FromDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1),
				ToDate = DateTime.Today
			};
			return View(vm);
		}

		// POST: /MaterialReport/Index
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Index(MaterialReportVM vm)
		{
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
				var fromParam = new SqlParameter("@FromDate", vm.FromDate.Value.Date);
				var toParam = new SqlParameter("@ToDate", vm.ToDate.Value.Date);

				List<MaterialReportRow> rows = _db.MaterialReportRows
					.FromSqlRaw("EXEC sp_GetMaterialPurchaseReport @FromDate, @ToDate",
								fromParam, toParam)
					.ToList();

				vm.ReportData = rows;
				vm.GrossTotal = rows.Count > 0 ? rows[0].Gross : 0;
			}
			catch (Exception ex)
			{
				TempData["Error"] = "Error fetching report: " + ex.Message;
			}

			return View(vm);
		}
	}
}
