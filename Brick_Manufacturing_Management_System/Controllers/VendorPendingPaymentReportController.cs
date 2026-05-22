using Brick_Manufacturing_Management_System.DBContext;
using Brick_Manufacturing_Management_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Brick_Manufacturing_Management_System.Controllers
{
	public class VendorPendingPaymentReportController : Controller
	{
		private readonly BrickErpdbContext _ctx;

		public VendorPendingPaymentReportController(BrickErpdbContext context)
		{
			_ctx = context;
		}

		private bool IsLoggedIn() =>
			HttpContext.Session.GetString("Username") != null;

		[HttpGet]
		public async Task<IActionResult> Index()
		{
			if (!IsLoggedIn()) return RedirectToAction("Index", "Login");

			var data = await _ctx.Database
				.SqlQueryRaw<VendorPendingPaymentVM>("EXEC sp_GetVendorPendingPaymentReport")
				.ToListAsync();

			return View(data);
		}
	}
}