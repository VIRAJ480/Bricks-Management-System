using Brick_Manufacturing_Management_System.DBContext;
using Brick_Manufacturing_Management_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Brick_Manufacturing_Management_System.Controllers
{
	public class BrickProductionController : Controller
	{
		private readonly BrickErpdbContext _ctx;

		public BrickProductionController(BrickErpdbContext context)
		{
			_ctx = context;
		}

		private bool IsLoggedIn() =>
			HttpContext.Session.GetString("Username") != null;

		// ── Helper: brick type dropdown ───────────────────────────────────────
		private async Task PopulateDropdowns(BrickProductionVM vm)
		{
			vm.BrickTypeOptions = await _ctx.BrickTypes
				.Where(b => b.Status == true)
				.OrderBy(b => b.BrickTypeName)
				.Select(b => new SelectListItem
				{
					Value = b.BrickTypeId.ToString(),
					Text = b.BrickTypeName
				})
				.ToListAsync();
		}

		// ── Helper: flat list for table ───────────────────────────────────────
		private async Task<List<BrickProductionListItem>> GetProductionList()
		{
			return await _ctx.BrickProductions
				.Include(p => p.BrickType)
				.OrderByDescending(p => p.ProductionDate)
				.ThenByDescending(p => p.ProductionId)
				.Select(p => new BrickProductionListItem
				{
					ProductionId = p.ProductionId,
					ProductionDate = p.ProductionDate ?? DateOnly.FromDateTime(DateTime.Today),
					BrickTypeName = p.BrickType != null ? p.BrickType.BrickTypeName : "—",
					Quantity = p.Quantity ?? 0
				})
				.ToListAsync();
		}

		// ══ INDEX ─────────────────────────────────────────────────────────────
		[HttpGet]
		public async Task<IActionResult> Index(int? editId)
		{
			if (!IsLoggedIn()) return RedirectToAction("Index", "Login");

			var vm = new BrickProductionVM
			{
				ProductionDate = DateOnly.FromDateTime(DateTime.Today)
			};

			await PopulateDropdowns(vm);
			vm.ProductionList = await GetProductionList();

			if (editId.HasValue)
			{
				var entity = await _ctx.BrickProductions.FindAsync(editId.Value);
				if (entity != null)
				{
					vm.ProductionId = entity.ProductionId;
					vm.ProductionDate = entity.ProductionDate;   // DateOnly → DateOnly ✔
					vm.BrickTypeId = entity.BrickTypeId;
					vm.Quantity = entity.Quantity;
				}
			}

			return View(vm);
		}

		// ══ SAVE — Insert or Update ───────────────────────────────────────────
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Save(BrickProductionVM model)
		{
			if (!IsLoggedIn()) return RedirectToAction("Index", "Login");

			await PopulateDropdowns(model);
			model.ProductionList = await GetProductionList();

			if (!ModelState.IsValid)
				return View("Index", model);

			DateOnly productionDate = model.ProductionDate!.Value;

			if (model.ProductionId == 0)
			{
				// INSERT
				var entity = new BrickProduction
				{
					ProductionDate = productionDate,             // DateOnly → DateOnly ✔
					BrickTypeId = model.BrickTypeId,
					Quantity = model.Quantity
				};
				_ctx.BrickProductions.Add(entity);
				await _ctx.SaveChangesAsync();
				TempData["Success"] = $"{productionDate:dd MMM yyyy} रोजी {model.Quantity:N0} विटांचं उत्पादन यशस्वीरित्या नोंद झालं.";
			}
			else
			{
				// UPDATE
				var entity = await _ctx.BrickProductions.FindAsync(model.ProductionId);
				if (entity == null)
				{
					TempData["Error"] = "उत्पादनाची नोंद सापडली नाही.";
					return RedirectToAction(nameof(Index));
				}

				entity.ProductionDate = productionDate;          // DateOnly → DateOnly ✔
				entity.BrickTypeId = model.BrickTypeId;
				entity.Quantity = model.Quantity;

				await _ctx.SaveChangesAsync();
				TempData["Success"] = $"{productionDate:dd MMM yyyy} रोजी {model.Quantity:N0} विटांचं उत्पादन यशस्वीरित्या अपडेट झालं.";
			}

			return RedirectToAction(nameof(Index));
		}

		// ══ DELETE ────────────────────────────────────────────────────────────
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Delete(int id)
		{
			if (!IsLoggedIn()) return RedirectToAction("Index", "Login");

			var entity = await _ctx.BrickProductions.FindAsync(id);
			if (entity != null)
			{
				_ctx.BrickProductions.Remove(entity);
				await _ctx.SaveChangesAsync();
				TempData["Success"] = "उत्पादनाची नोंद डिलीट झाली.";
			}
			else
			{
				TempData["Error"] = "उत्पादनाची नोंद सापडली नाही.";
			}

			return RedirectToAction(nameof(Index));
		}
	}
}
