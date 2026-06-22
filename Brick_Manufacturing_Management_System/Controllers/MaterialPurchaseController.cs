using Brick_Manufacturing_Management_System.DBContext;
using Brick_Manufacturing_Management_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Brick_Manufacturing_Management_System.Controllers
{
	public class MaterialPurchaseController : Controller
	{
		private readonly BrickErpdbContext _ctx;

		public MaterialPurchaseController(BrickErpdbContext context)
		{
			_ctx = context;
		}

		private bool IsLoggedIn() =>
			HttpContext.Session.GetString("Username") != null;

		// ── Helper: build dropdown options ───────────────────────────────────
		private async Task PopulateDropdowns(MaterialPurchaseVM vm)
		{
			vm.VendorOptions = await _ctx.VendorMasters
				.Where(v => v.Status == true)
				.OrderBy(v => v.VendorName)
				.Select(v => new SelectListItem
				{
					Value = v.VendorId.ToString(),
					Text = v.VendorName
				})
				.ToListAsync();

			vm.MaterialOptions = await _ctx.MaterialMasters
				.Where(m => m.Status == true)
				.OrderBy(m => m.MaterialName)
				.Select(m => new SelectListItem
				{
					Value = m.MaterialId.ToString(),
					Text = m.MaterialName
				})
				.ToListAsync();
		}

		// ── Helper: build flat purchase list ─────────────────────────────────
		private async Task<List<MaterialPurchaseListItem>> GetPurchaseList()
		{
			return await _ctx.MaterialPurchases
				.Include(p => p.Vendor)
				.Include(p => p.Material)
				.OrderByDescending(p => p.PurchaseDate)
				.ThenByDescending(p => p.PurchaseId)
				.Select(p => new MaterialPurchaseListItem
				{
					PurchaseId = p.PurchaseId,
					PurchaseDate = p.PurchaseDate,           // DateOnly → DateOnly ✔
					VendorName = p.Vendor != null ? p.Vendor.VendorName : "—",
					MaterialName = p.Material != null ? p.Material.MaterialName ?? "—" : "—",
					Quantity = p.Quantity ?? 0,
					Rate = p.Rate ?? 0,
					TotalAmount = p.TotalAmount ?? 0,
					PaidAmount = p.PaidAmount ?? 0,
					PendingAmount = p.PendingAmount ?? 0
				})
				.ToListAsync();
		}

		// ══ INDEX ─────────────────────────────────────────────────────────────
		[HttpGet]
		public async Task<IActionResult> Index(int? editId)
		{
			 if (!IsLoggedIn()) return RedirectToAction("Index", "Login");

			var vm = new MaterialPurchaseVM
			{
				PurchaseDate = DateOnly.FromDateTime(DateTime.Today)
			};

			await PopulateDropdowns(vm);
			vm.PurchaseList = await GetPurchaseList();

			if (editId.HasValue)
			{
				var entity = await _ctx.MaterialPurchases.FindAsync(editId.Value);
				if (entity != null)
				{
					vm.PurchaseId = entity.PurchaseId;
					vm.PurchaseDate = entity.PurchaseDate;   // DateOnly → DateOnly ✔
					vm.VendorId = entity.VendorId;
					vm.MaterialId = entity.MaterialId;
					vm.Quantity = entity.Quantity;
					vm.Rate = entity.Rate;
					vm.TotalAmount = entity.TotalAmount;
					vm.PaidAmount = entity.PaidAmount;
					vm.PendingAmount = entity.PendingAmount;
				}
			}

			return View(vm);
		}

		// ══ SAVE — Insert or Update ───────────────────────────────────────────
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Save(MaterialPurchaseVM model)
		{
			if (!IsLoggedIn()) return RedirectToAction("Index", "Login");

			// Auto-calculate before validation
			decimal qty = model.Quantity ?? 0;
			decimal rate = model.Rate ?? 0;
			decimal paid = model.PaidAmount ?? 0;
			decimal total = qty * rate;
			decimal pending = total - paid;

			model.TotalAmount = total;
			model.PendingAmount = pending;

			// Rebuild for re-render
			await PopulateDropdowns(model);
			model.PurchaseList = await GetPurchaseList();

			// Paid cannot exceed total
			if (paid > total)
			{
				ModelState.AddModelError("PaidAmount", "दिलेली रक्कम एकूण रकमेपेक्षा जास्त असू शकत नाही.");
				return View("Index", model);
			}

			if (!ModelState.IsValid)
				return View("Index", model);

			// Convert DateOnly? → DateOnly (safe because [Required] passed)
			DateOnly purchaseDate = model.PurchaseDate!.Value;

			if (model.PurchaseId == 0)
			{
				// INSERT
				var entity = new MaterialPurchase
				{
					PurchaseDate = purchaseDate,   // DateOnly → DateOnly ✔
					VendorId = model.VendorId,
					MaterialId = model.MaterialId,
					Quantity = qty,
					Rate = rate,
					TotalAmount = total,
					PaidAmount = paid,
					PendingAmount = pending
				};
				_ctx.MaterialPurchases.Add(entity);
				await _ctx.SaveChangesAsync();
				TempData["Success"] = $"खरेदी यशस्वीरित्या नोंद झाली. एकूण रक्कम: ₹{total:N2}";
			}
			else
			{
				// UPDATE
				var entity = await _ctx.MaterialPurchases.FindAsync(model.PurchaseId);
				if (entity == null)
				{
					TempData["Error"] = "Purchase record सापडलं नाही.";
					return RedirectToAction(nameof(Index));
				}

				entity.PurchaseDate = purchaseDate;   // DateOnly → DateOnly ✔
				entity.VendorId = model.VendorId;
				entity.MaterialId = model.MaterialId;
				entity.Quantity = qty;
				entity.Rate = rate;
				entity.TotalAmount = total;
				entity.PaidAmount = paid;
				entity.PendingAmount = pending;
				await _ctx.SaveChangesAsync();
				TempData["Success"] = $"खरेदी यशस्वीरित्या अपडेट झाली. एकूण रक्कम: ₹{total:N2}";
			}

			return RedirectToAction(nameof(Index));
		}

		// ══ DELETE ───────────────────────────────────────────────────────────
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Delete(int id)
		{
			if (!IsLoggedIn()) return RedirectToAction("Index", "Login");

			var entity = await _ctx.MaterialPurchases.FindAsync(id);
			if (entity != null)
			{
				_ctx.MaterialPurchases.Remove(entity);
				await _ctx.SaveChangesAsync();
				TempData["Success"] = "खरेदीची नोंद डिलीट झाली.";
			}
			else
			{
				TempData["Error"] = "खरेदीची नोंद सापडली नाही.";
			}

			return RedirectToAction(nameof(Index));
		}
	}
}
