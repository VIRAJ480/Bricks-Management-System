using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Brick_Manufacturing_Management_System.Models;
using Brick_Manufacturing_Management_System.DBContext;

namespace Brick_Manufacturing_Management_System.Controllers
{
	public class VendorSpecificMaterialPurchaseController : Controller
	{
		private readonly BrickErpdbContext _db;

		public VendorSpecificMaterialPurchaseController(BrickErpdbContext db)
		{
			_db = db;
		}

		private bool IsLoggedIn() =>
			HttpContext.Session.GetString("Username") != null;

		private List<SelectListItem> GetVendorOptions() =>
			_db.VendorMasters
				.OrderBy(v => v.VendorName)
				.Select(v => new SelectListItem
				{
					Value = v.VendorId.ToString(),
					Text = v.VendorName
				})
				.ToList();

		// GET: /VendorSpecificMaterialPurchase/Index
		public IActionResult Index()
		{
			if (!IsLoggedIn()) return RedirectToAction("Index", "Login");

			var vm = new VendorSpecificMaterialPurchaseVM
			{
				FromDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1),
				ToDate = DateTime.Today,
				VendorOptions = GetVendorOptions()
			};
			return View(vm);
		}

		// POST: /VendorSpecificMaterialPurchase/Index
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Index(VendorSpecificMaterialPurchaseVM vm)
		{
			if (!IsLoggedIn()) return RedirectToAction("Index", "Login");

			vm.VendorOptions = GetVendorOptions();

			if (vm.VendorId == null || vm.VendorId == 0)
			{
				TempData["Error"] = "Please select a vendor.";
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

				// Selected vendor name (shown in report / print)
				vm.VendorName = _db.VendorMasters
					.Where(v => v.VendorId == vm.VendorId)
					.Select(v => v.VendorName)
					.FirstOrDefault() ?? "—";

				var purchases = _db.MaterialPurchases
					.Include(p => p.Vendor)
					.Include(p => p.Material)
					.Where(p => p.VendorId == vm.VendorId
								&& p.PurchaseDate >= fromD && p.PurchaseDate <= toD)
					.OrderBy(p => p.PurchaseDate)
					.ThenBy(p => p.PurchaseId)
					.ToList();

				var rows = purchases.Select(p => new VendorSpecificMaterialPurchaseRow
				{
					PurchaseDate = p.PurchaseDate.ToDateTime(TimeOnly.MinValue),
					VendorName   = p.Vendor != null ? (p.Vendor.VendorName ?? "—") : "—",
					MaterialName = p.Material != null ? (p.Material.MaterialName ?? "—") : "—",
					Quantity     = p.Quantity ?? 0,
					Rate         = p.Rate ?? 0,
					TotalAmount  = p.TotalAmount ?? 0
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
