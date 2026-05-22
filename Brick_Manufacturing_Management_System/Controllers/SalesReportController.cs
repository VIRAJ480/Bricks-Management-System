using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Brick_Manufacturing_Management_System.Models;
using Brick_Manufacturing_Management_System.DBContext;

namespace Brick_Manufacturing_Management_System.Controllers
{
	public class SalesReportController : Controller
	{
		private readonly BrickErpdbContext _db;

		public SalesReportController(BrickErpdbContext db)
		{
			_db = db;
		}

		// GET: /SalesReport/Index
		public IActionResult Index()
		{
			var vm = new SalesReportVM
			{
				FromDate = new DateTime(DateTime.Today.Year, 1, 1),
				ToDate = DateTime.Today,
				IsGenerated = false
			};
			return View(vm);
		}

		// POST: /SalesReport/Generate
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Generate(SalesReportVM vm)
		{
			if (vm.FromDate == default) vm.FromDate = new DateTime(DateTime.Today.Year, 1, 1);
			if (vm.ToDate == default) vm.ToDate = DateTime.Today;

			var fromParam = new SqlParameter("@FromDate", vm.FromDate.ToString("yyyy-MM-dd"));
			var toParam = new SqlParameter("@ToDate", vm.ToDate.ToString("yyyy-MM-dd"));

			var result = _db.SalesReportSpResults
							.FromSqlRaw("EXEC sp_GetSalesDashboardReport @FromDate, @ToDate",
										fromParam, toParam)
							.AsEnumerable()
							.FirstOrDefault();

			if (result != null)
			{
				vm.TotalCustomers = result.TotalCustomers;
				vm.TotalRevenue = result.TotalRevenue;
				vm.ReportGeneratedOn = result.ReportGeneratedOn;
			}

			vm.IsGenerated = true;

			return View("Index", vm);
		}
	}
}
