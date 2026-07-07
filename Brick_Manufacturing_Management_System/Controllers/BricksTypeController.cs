using Brick_Manufacturing_Management_System.DBContext;
using Brick_Manufacturing_Management_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Brick_Manufacturing_Management_System.Controllers
{
	public class BricksTypeController : Controller
	{
		private readonly BrickErpdbContext _ctx;

		public BricksTypeController(BrickErpdbContext context)
		{
			_ctx = context;
		}

		private bool IsLoggedIn() =>
			HttpContext.Session.GetString("Username") != null;

		private async Task<BricksTypeVM> BuildVM(BricksTypeVM? form = null)
		{
			var vm = form ?? new BricksTypeVM { Status = true };
			vm.BrickTypeList = await _ctx.BrickTypes
				.OrderByDescending(b => b.BrickTypeId)
				.ToListAsync();
			return vm;
		}

		[HttpGet]
		public async Task<IActionResult> Index(int? editId)
		{
			if (!IsLoggedIn()) return RedirectToAction("Index", "Login");

			var vm = await BuildVM();

			if (editId.HasValue)
			{
				var entity = vm.BrickTypeList.FirstOrDefault(b => b.BrickTypeId == editId.Value);
				if (entity != null)
				{
					vm.BrickTypeId = entity.BrickTypeId;
					vm.BrickTypeName = entity.BrickTypeName;
					vm.Status = entity.Status ?? true;
				}
			}

			return View(vm);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Save(BricksTypeVM model)
		{
			if (!IsLoggedIn()) return RedirectToAction("Index", "Login");

			var rebuilt = await BuildVM(model);

			if (!ModelState.IsValid)
				return View("Index", rebuilt);

			bool duplicate = await _ctx.BrickTypes.AnyAsync(b =>
				b.BrickTypeName.ToLower() == model.BrickTypeName.Trim().ToLower() &&
				b.BrickTypeId != model.BrickTypeId);

			if (duplicate)
			{
				ModelState.AddModelError("BrickTypeName", "या नावाचा ब्रिक टाईप आधीपासून आहे.");
				return View("Index", rebuilt);
			}

			bool statusVal = model.Status ?? true;

			if (model.BrickTypeId == 0)
			{
				var entity = new BrickType
				{
					BrickTypeName = model.BrickTypeName.Trim(),
					Status = statusVal
				};
				_ctx.BrickTypes.Add(entity);
				await _ctx.SaveChangesAsync();
				TempData["Success"] = $"'{entity.BrickTypeName}' ब्रिक टाईप यशस्वीरित्या अॅड झाला.";
			}
			else
			{
				var entity = await _ctx.BrickTypes.FindAsync(model.BrickTypeId);
				if (entity == null)
				{
					TempData["Error"] = "Brick Type not found.";
					return RedirectToAction(nameof(Index));
				}
				entity.BrickTypeName = model.BrickTypeName.Trim();
				entity.Status = statusVal;
				await _ctx.SaveChangesAsync();
				TempData["Success"] = $"'{entity.BrickTypeName}' ब्रिक टाईप यशस्वीरित्या अपडेट झाला.";
			}

			return RedirectToAction(nameof(Index));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Delete(int id)
		{
			if (!IsLoggedIn()) return RedirectToAction("Index", "Login");

			var entity = await _ctx.BrickTypes.FindAsync(id);
			if (entity == null)
			{
				TempData["Error"] = "Brick Type not found.";
				return RedirectToAction(nameof(Index));
			}

			bool hasProductions = await _ctx.BrickProductions.AnyAsync(x => x.BrickTypeId == id);
			if (hasProductions)
			{
				TempData["Error"] = $"'{entity.BrickTypeName}' डिलीट करता येणार नाही — Brick Production मध्ये याचे रेकॉर्ड आधीपासून आहेत.";
				return RedirectToAction(nameof(Index));
			}
		
			bool hasSales = await _ctx.BrickSales.AnyAsync(x => x.BrickTypeId == id);
			if (hasSales)
			{
				TempData["Error"] = $"'{entity.BrickTypeName}' डिलीट करता येणार नाही — Brick Sales मध्ये याचे रेकॉर्ड आधीपासून आहेत. आधी ते रेकॉर्ड डिलीट करा.";
				return RedirectToAction(nameof(Index));
			}
			_ctx.BrickTypes.Remove(entity);
			await _ctx.SaveChangesAsync();
			TempData["Success"] = $"'{entity.BrickTypeName}' ब्रिक टाईप डिलीट झाला.";

			return RedirectToAction(nameof(Index));
		}

		[HttpPost]
		public async Task<IActionResult> ToggleStatus(int id)
		{
			if (!IsLoggedIn())
				return Json(new { success = false });

			var entity = await _ctx.BrickTypes.FindAsync(id);
			if (entity == null)
				return Json(new { success = false });

			entity.Status = !(entity.Status ?? false);
			await _ctx.SaveChangesAsync();
			return Json(new { success = true, status = entity.Status });
		}
	}
}