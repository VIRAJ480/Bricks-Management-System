using Brick_Manufacturing_Management_System.DBContext;
using Brick_Manufacturing_Management_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Brick_Manufacturing_Management_System.Controllers
{
	public class MaterialController : Controller
	{
		private readonly BrickErpdbContext _ctx;

		public MaterialController(BrickErpdbContext context)
		{
			_ctx = context;
		}

		private bool IsLoggedIn() =>
			HttpContext.Session.GetString("Username") != null;

		// ── Helper: build a fresh VM with list + unit dropdown ────────────────
		private async Task<MaterialVM> BuildVM(MaterialVM? form = null)
		{
			var vm = form ?? new MaterialVM { Status = true };

			vm.MaterialList = await _ctx.MaterialMasters
				.Include(m => m.Unit)                        // navigation property
				.OrderByDescending(m => m.MaterialId)
				.ToListAsync();

			vm.UnitOptions = await _ctx.Units
				.OrderBy(u => u.UnitName)
				.Select(u => new SelectListItem
				{
					Value = u.UnitId.ToString(),
					Text = u.UnitName
				})
				.ToListAsync();

			return vm;
		}

		// ══ INDEX — single page with form + table ═════════════════════════════
		[HttpGet]
		public async Task<IActionResult> Index(int? editId)
		{
			if (!IsLoggedIn()) return RedirectToAction("Index", "Login");

			var vm = await BuildVM();

			if (editId.HasValue)
			{
				var entity = vm.MaterialList.FirstOrDefault(m => m.MaterialId == editId.Value);
				if (entity != null)
				{
					vm.MaterialId = entity.MaterialId;
					vm.MaterialName = entity.MaterialName ?? string.Empty;
					vm.UnitId = entity.UnitId;
					vm.Status = entity.Status ?? true;
				}
			}

			return View(vm);
		}

		// ══ SAVE — Insert or Update ═══════════════════════════════════════════
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Save(MaterialVM model)
		{
			if (!IsLoggedIn()) return RedirectToAction("Index", "Login");

			// Rebuild list + dropdown for any re-render
			var rebuilt = await BuildVM(model);

			if (!ModelState.IsValid)
				return View("Index", rebuilt);

			// Duplicate name check (case-insensitive, exclude self on edit)
			bool duplicate = await _ctx.MaterialMasters.AnyAsync(m =>
				m.MaterialName!.ToLower() == model.MaterialName.Trim().ToLower() &&
				m.MaterialId != model.MaterialId);

			if (duplicate)
			{
				ModelState.AddModelError("MaterialName", "A material with this name already exists.");
				return View("Index", rebuilt);
			}

			bool statusVal = model.Status ?? true;

			if (model.MaterialId == 0)
			{
				// INSERT
				var entity = new MaterialMaster
				{
					MaterialName = model.MaterialName.Trim(),
					UnitId = model.UnitId,
					Status = statusVal
				};
				_ctx.MaterialMasters.Add(entity);
				await _ctx.SaveChangesAsync();
				TempData["Success"] = $"Material '{entity.MaterialName}' added successfully.";
			}
			else
			{
				// UPDATE
				var entity = await _ctx.MaterialMasters.FindAsync(model.MaterialId);
				if (entity == null)
				{
					TempData["Error"] = "Material not found.";
					return RedirectToAction(nameof(Index));
				}
				entity.MaterialName = model.MaterialName.Trim();
				entity.UnitId = model.UnitId;
				entity.Status = statusVal;
				await _ctx.SaveChangesAsync();
				TempData["Success"] = $"Material '{entity.MaterialName}' updated successfully.";
			}

			return RedirectToAction(nameof(Index));
		}

		// ══ DELETE ════════════════════════════════════════════════════════════
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Delete(int id)
		{
			if (!IsLoggedIn()) return RedirectToAction("Index", "Login");

			var entity = await _ctx.MaterialMasters.FindAsync(id);
			if (entity == null)
			{
				TempData["Error"] = "Material not found.";
				return RedirectToAction(nameof(Index));
			}

			bool hasPurchases = await _ctx.MaterialPurchases.AnyAsync(x => x.MaterialId == id);
			if (hasPurchases)
			{
				TempData["Error"] = $"Cannot delete '{entity.MaterialName}' — linked records exist in: Material Purchase. Please delete those records first.";
				return RedirectToAction(nameof(Index));
			}

			_ctx.MaterialMasters.Remove(entity);
			await _ctx.SaveChangesAsync();
			TempData["Success"] = $"Material '{entity.MaterialName}' deleted.";

			return RedirectToAction(nameof(Index));
		}

		// ══ TOGGLE STATUS (AJAX) ══════════════════════════════════════════════
		[HttpPost]
		public async Task<IActionResult> ToggleStatus(int id)
		{
			if (!IsLoggedIn())
				return Json(new { success = false });

			var entity = await _ctx.MaterialMasters.FindAsync(id);
			if (entity == null)
				return Json(new { success = false });

			entity.Status = !(entity.Status ?? false);   // null-safe toggle
			await _ctx.SaveChangesAsync();
			return Json(new { success = true, status = entity.Status });
		}
	}
}
