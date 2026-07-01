using Brick_Manufacturing_Management_System.DBContext;
using Brick_Manufacturing_Management_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Brick_Manufacturing_Management_System.Controllers
{
	public class VendorController : Controller
	{
		private readonly BrickErpdbContext _ctx;

		public VendorController(BrickErpdbContext context)
		{
			_ctx = context;
		}

		private bool IsLoggedIn() =>
			HttpContext.Session.GetString("Username") != null;

		// ══ INDEX — Single page with form + list ═════════════════════════════
		[HttpGet]
		public async Task<IActionResult> Index(int? editId)
		{
			if (!IsLoggedIn()) return RedirectToAction("Index", "Login");

			var list = await _ctx.VendorMasters
				.OrderByDescending(v => v.VendorId)
				.ToListAsync();

			var vm = new VendorVM { VendorList = list, Status = true };

			if (editId.HasValue)
			{
				var entity = list.FirstOrDefault(v => v.VendorId == editId.Value);
				if (entity != null)
				{
					vm.VendorId = entity.VendorId;
					vm.VendorName = entity.VendorName;
					vm.MobileNumber = entity.MobileNumber;
					vm.Address = entity.Address;
					vm.GSTNumber = entity.Gstnumber;
					vm.Status = entity.Status ?? true;   // null-safe
				}
			}

			return View(vm);
		}

		// ══ SAVE — Insert or Update ══════════════════════════════════════════
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Save(VendorVM model)
		{
			if (!IsLoggedIn()) return RedirectToAction("Index", "Login");

			model.VendorList = await _ctx.VendorMasters
				.OrderByDescending(v => v.VendorId)
				.ToListAsync();

			if (!ModelState.IsValid)
				return View("Index", model);

			// Duplicate name check
			bool dupName = await _ctx.VendorMasters.AnyAsync(v =>
				v.VendorName.ToLower() == model.VendorName.Trim().ToLower() &&
				v.VendorId != model.VendorId);

			if (dupName)
			{
				ModelState.AddModelError("VendorName", "A vendor with this name already exists.");
				return View("Index", model);
			}

			// Duplicate GST check
			if (!string.IsNullOrWhiteSpace(model.GSTNumber))
			{
				bool dup = await _ctx.VendorMasters.AnyAsync(v =>
					v.Gstnumber == model.GSTNumber.Trim().ToUpper() &&
					v.VendorId != model.VendorId);

				if (dup)
				{
					ModelState.AddModelError("GSTNumber", "A vendor with this GST number already exists.");
					return View("Index", model);
				}
			}

			bool statusValue = model.Status ?? true;   // null-safe default

			if (model.VendorId == 0)
			{
				// INSERT
				var entity = new VendorMaster
				{
					VendorName = model.VendorName.Trim(),
					MobileNumber = model.MobileNumber?.Trim(),
					Address = model.Address?.Trim(),
					Gstnumber = model.GSTNumber?.Trim().ToUpper(),
					Status = statusValue
				};
				_ctx.VendorMasters.Add(entity);
				await _ctx.SaveChangesAsync();
				TempData["Success"] = $"Vendor '{entity.VendorName}' added successfully.";
			}
			else
			{
				// UPDATE
				var entity = await _ctx.VendorMasters.FindAsync(model.VendorId);
				if (entity == null)
				{
					TempData["Error"] = "Vendor not found.";
					return RedirectToAction(nameof(Index));
				}
				entity.VendorName = model.VendorName.Trim();
				entity.MobileNumber = model.MobileNumber?.Trim();
				entity.Address = model.Address?.Trim();
				entity.Gstnumber = model.GSTNumber?.Trim().ToUpper();
				entity.Status = statusValue;
				await _ctx.SaveChangesAsync();
				TempData["Success"] = $"Vendor '{entity.VendorName}' updated successfully.";
			}

			return RedirectToAction(nameof(Index));
		}

		// ══ DELETE ═══════════════════════════════════════════════════════════
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Delete(int id)
		{
			if (!IsLoggedIn()) return RedirectToAction("Index", "Login");

			var entity = await _ctx.VendorMasters.FindAsync(id);
			if (entity == null)
			{
				TempData["Error"] = "Vendor not found.";
				return RedirectToAction(nameof(Index));
			}

			bool hasPurchases = await _ctx.MaterialPurchases.AnyAsync(x => x.VendorId == id);
			if (hasPurchases)
			{
				TempData["Error"] = $"'{entity.VendorName}' ला डिलीट करता येणार नाही — Material Purchase मध्ये याचे रेकॉर्ड आधीपासून आहेत. आधी ते रेकॉर्ड डिलीट करा.";
				return RedirectToAction(nameof(Index));
			}

			// Check Vendor Payment
			bool hasVendorPayment = await _ctx.VendorPayments
				.AnyAsync(x => x.VendorId == id);

			if (hasVendorPayment)
			{
				TempData["Error"] = $"'{entity.VendorName}' ला डिलीट करता येणार नाही — Vendor Payment मध्ये याचे हिशोब आधीपासून आहेत.";
				//$"Cannot delete '{entity.VendorName}' — linked records exist in Vendor Payment.";

				return RedirectToAction(nameof(Index));
			}


			_ctx.VendorMasters.Remove(entity);
			await _ctx.SaveChangesAsync();
			TempData["Success"] = $"Vendor '{entity.VendorName}' deleted.";

			return RedirectToAction(nameof(Index));
		}

		// ══ TOGGLE STATUS (AJAX) ═════════════════════════════════════════════
		[HttpPost]
		public async Task<IActionResult> ToggleStatus(int id)
		{
			if (!IsLoggedIn())
				return Json(new { success = false });

			var entity = await _ctx.VendorMasters.FindAsync(id);
			if (entity == null)
				return Json(new { success = false });

			entity.Status = !(entity.Status ?? false);   // null-safe toggle
			await _ctx.SaveChangesAsync();
			return Json(new { success = true, status = entity.Status });
		}
	}
}
