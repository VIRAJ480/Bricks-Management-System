using Brick_Manufacturing_Management_System.DBContext;
using Brick_Manufacturing_Management_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Brick_Manufacturing_Management_System.Controllers
{
	public class LoginController : Controller
	{
		private readonly BrickErpdbContext ctx;

		public LoginController(BrickErpdbContext context)
		{
			ctx = context;
		}

		[HttpGet]
		public IActionResult Index()
		{
			if (HttpContext.Session.GetString("Username") != null)
				return RedirectToAction("Dashboard", "Home");

			return View(new LoginVM());
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Index(LoginVM model)
		{
			if (!ModelState.IsValid)
				return View(model);

			var user = ctx.Users
				.FirstOrDefault(u => u.Username == model.Username.Trim()
								  && u.Password == model.Password);

			if (user != null)
			{
				HttpContext.Session.SetString("Username", user.Username);
				HttpContext.Session.SetInt32("UserId", user.UserId);

				if (model.RememberMe)
				{
					Response.Cookies.Append("BrickERP_User", user.Username, new CookieOptions
					{
						Expires = DateTimeOffset.UtcNow.AddDays(14),
						HttpOnly = true,
						Secure = true,
						SameSite = SameSiteMode.Strict
					});
				}

				TempData["Success"] = $"Welcome back, {user.Username}!";
				return RedirectToAction("Dashboard", "Login");
			}

			TempData["LoginError"] = "Invalid username or password. Please try again.";
			return View(model);
		}

		[HttpGet]
		public IActionResult Logout()
		{
			HttpContext.Session.Clear();
			Response.Cookies.Delete("BrickERP_User");
			return RedirectToAction("Index", "Login");
		}

		[HttpGet]
		public async Task<IActionResult> Dashboard()
		{
			if (HttpContext.Session.GetString("Username") == null)
				return RedirectToAction("Index", "Login");

			var today        = DateOnly.FromDateTime(DateTime.Today);
			var firstOfMonth = new DateOnly(today.Year, today.Month, 1);

			// ── STAT CARDS ────────────────────────────────────────────────
			ViewBag.VendorCount       = await ctx.VendorMasters.CountAsync();
			ViewBag.MaterialCount     = await ctx.MaterialMasters.CountAsync();
			ViewBag.LabourCount       = await ctx.LabourMasters.CountAsync();
			//ViewBag.BricksToday       = await ctx.BrickProductions
			//	.Where(p => p.ProductionDate == today)
			//	.SumAsync(p => (decimal?)(p.Quantity ?? 0)) ?? 0;
			ViewBag.CustomerCount     = await ctx.CustomerMasters.CountAsync();
			ViewBag.SalesThisMonth    = await ctx.BrickSales
				.Where(s => s.SalesDate >= firstOfMonth && s.SalesDate <= today)
				.CountAsync();
			ViewBag.LabourLedgerCount = await ctx.LabourMasters.CountAsync();
			var breadBrickId = (await ctx.BrickTypes
				.Where(b => b.BrickTypeName == "Bread Brick" || b.BrickTypeName == "ब्रेड विट")
				.Select(b => b.BrickTypeId)
				.FirstOrDefaultAsync()).ToString();
			ViewBag.Breadbricks       = await ctx.BrickSales
				.Where(s => s.BrickType == breadBrickId)
				.SumAsync(s => (int?)s.Quantity) ?? 0;
			var solidBrickId = (await ctx.BrickTypes
				.Where(b => b.BrickTypeName == "Solid Brick" || b.BrickTypeName== "ठोकळा विट")
				.Select(b => b.BrickTypeId)
				.FirstOrDefaultAsync()).ToString();
			ViewBag.Solidbricks       = await ctx.BrickSales
				.Where(s => s.BrickType == solidBrickId)
				.SumAsync(s => (int?)s.Quantity) ?? 0;
			var Brokenbricksid = (await ctx.BrickTypes
		.Where(b => b.BrickTypeName == "Broken bricks" || b.BrickTypeName == "तुकडा विट")
		.Select(b => b.BrickTypeId)
		.FirstOrDefaultAsync()).ToString();
			ViewBag.Brokenbricks = await ctx.BrickSales
				.Where(s => s.BrickType == Brokenbricksid)
				.SumAsync(s => (int?)s.Quantity) ?? 0;

			// Pending Payments — replicate sp_GetPendingCustomerPayments logic
			var salesByCustomer = await ctx.BrickSales
				.GroupBy(s => s.CustomerId)
				.Select(g => new { CustomerId = g.Key, TotalSales = g.Sum(s => s.TotalAmount ?? 0) })
				.ToListAsync();
			var paidByCustomer = await ctx.CustomerPayments
				.GroupBy(p => p.CustomerId)
				.Select(g => new { CustomerId = g.Key, TotalPaid = g.Sum(p => p.Amount ?? 0) })
				.ToListAsync();
			var paidLookup = paidByCustomer.ToDictionary(p => p.CustomerId, p => p.TotalPaid);
			ViewBag.PendingPayments = salesByCustomer
				.Sum(s => { var r = s.TotalSales - (paidLookup.ContainsKey(s.CustomerId) ? paidLookup[s.CustomerId] : 0); return r > 0 ? r : 0; });

			// Vendor Pending Payments — same logic: MaterialPurchases total per vendor minus VendorPayments total per vendor
			var purchasesByVendor = await ctx.MaterialPurchases
				.Where(p => p.VendorId != null)
				.GroupBy(p => p.VendorId)
				.Select(g => new { VendorId = g.Key, TotalPurchases = g.Sum(p => p.TotalAmount ?? 0) })
				.ToListAsync();
			var paidByVendor = await ctx.VendorPayments
				.GroupBy(p => p.VendorId)
				.Select(g => new { VendorId = g.Key, TotalPaid = g.Sum(p => p.Amount) })
				.ToListAsync();
			var vendorPaidLookup = paidByVendor.ToDictionary(p => (int?)p.VendorId, p => p.TotalPaid);
			ViewBag.VendorPendingPayments = purchasesByVendor
				.Sum(p => { var r = p.TotalPurchases - (vendorPaidLookup.ContainsKey(p.VendorId) ? vendorPaidLookup[p.VendorId] : 0); return r > 0 ? r : 0; });

			ViewBag.AdvanceBalance = await ctx.AdvanceDeductions
				.SumAsync(d => (decimal?)d.Amount) ?? 0;

			// ── RECENT BRICK SALES (top 5) ────────────────────────────────
			ViewBag.RecentSales = await ctx.BrickSales
				.Include(s => s.Customer)
				.OrderByDescending(s => s.SalesDate)
				.ThenByDescending(s => s.SalesId)
				.Take(5)
				.Select(s => new {
					s.SalesId,
					CustomerName  = s.Customer != null ? s.Customer.CustomerName ?? "—" : "—",
					s.Quantity,
					s.Rate,
					s.TotalAmount,
					s.PaidAmount,
					s.PendingAmount
				})
				.ToListAsync();

			// ── RECENT MATERIAL PURCHASES (top 5) ────────────────────────
			ViewBag.RecentPurchases = await ctx.MaterialPurchases
				.Include(p => p.Vendor)
				.Include(p => p.Material)
				.OrderByDescending(p => p.PurchaseDate)
				.ThenByDescending(p => p.PurchaseId)
				.Take(5)
				.Select(p => new {
					p.PurchaseId,
					VendorName   = p.Vendor   != null ? p.Vendor.VendorName          : "—",
					MaterialName = p.Material != null ? p.Material.MaterialName ?? "—" : "—",
					p.Quantity,
					p.TotalAmount,
					p.PaidAmount,
					p.PendingAmount
				})
				.ToListAsync();

			// ── LABOUR OVERVIEW (top 5 most recent) ──────────────────────
			ViewBag.RecentLabour = await ctx.LabourMasters
				.OrderByDescending(l => l.LabourId)
				.Take(5)
				.Select(l => new {
					l.LabourId,
					l.LabourName,
					l.DailyWage,
					l.JoiningDate
				})
				.ToListAsync();

			// ── ADVANCE SUMMARY ───────────────────────────────────────────
			ViewBag.TotalAdvancesThisMonth = await ctx.LabourAdvances
				.Where(a => a.AdvanceDate >= firstOfMonth && a.AdvanceDate <= today)
				.SumAsync(a => (decimal?)a.AdvanceAmount) ?? 0;
			ViewBag.TotalDeductionsThisMonth = await ctx.AdvanceDeductions
				.Where(d => d.DeductionDate >= firstOfMonth && d.DeductionDate <= today)
				.SumAsync(d => (decimal?)d.Amount) ?? 0;
			ViewBag.OutstandingAdvance =
				(await ctx.LabourAdvances.SumAsync(a => (decimal?)a.AdvanceAmount) ?? 0)
				- (await ctx.AdvanceDeductions.SumAsync(d => (decimal?)d.Amount) ?? 0);

			// ── PRODUCTION (last 5 days with data) ───────────────────────
			ViewBag.RecentProduction = await ctx.BrickProductions
				.GroupBy(p => p.ProductionDate)
				.Select(g => new {
					Date     = g.Key,
					Quantity = g.Sum(p => p.Quantity ?? 0)
				})
				.OrderByDescending(g => g.Date)
				.Take(5)
				.ToListAsync();

			// ── SALARY STATUS (this month) ────────────────────────────────
			ViewBag.SalaryPaidCount    = await ctx.SalaryPayments
				.Where(s => s.StatusId == 1 && s.SalaryDate >= firstOfMonth && s.SalaryDate <= today)
				.CountAsync();
			ViewBag.SalaryPendingCount = await ctx.SalaryPayments
				.Where(s => s.StatusId != 1 && s.SalaryDate >= firstOfMonth && s.SalaryDate <= today)
				.CountAsync();
			ViewBag.TotalSalaryPaidOut = await ctx.SalaryPayments
				.Where(s => s.SalaryDate >= firstOfMonth && s.SalaryDate <= today)
				.SumAsync(s => (decimal?)s.PaidAmount) ?? 0;

			// ── PENDING CUSTOMER DUES (top 3 from SP) ────────────────────
			ViewBag.PendingDues = (await ctx.Database
				.SqlQueryRaw<PendingPaymentsVM>("EXEC sp_GetPendingCustomerPayments")
				.ToListAsync())
				.Take(3)
				.ToList();

			ViewBag.Username = HttpContext.Session.GetString("Username");
			return View();
		}
	}
}
