using Brick_Manufacturing_Management_System.DBContext;
using Brick_Manufacturing_Management_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Brick_Manufacturing_Management_System.Controllers
{
	public class PendingPaymentReportController : Controller
	{
		private readonly BrickErpdbContext _ctx;

		public PendingPaymentReportController(BrickErpdbContext context)
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
				.SqlQueryRaw<PendingPaymentsVM>("EXEC sp_GetPendingCustomerPayments")
				.ToListAsync();

			return View(data);
		}
	}
}
