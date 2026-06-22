using Brick_Manufacturing_Management_System.DBContext;
using Brick_Manufacturing_Management_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Brick_Manufacturing_Management_System.Controllers
{
	public class LabourAdvanceController : Controller
	{
		private readonly BrickErpdbContext _ctx;

		public LabourAdvanceController(BrickErpdbContext context)
		{
			_ctx = context;
		}

		private bool IsLoggedIn() =>
			HttpContext.Session.GetString("Username") != null;

		// ══ Helpers ══════════════════════════════════════════════════
		private async Task<List<SelectListItem>> GetLabourOptions() =>
			await _ctx.LabourMasters
				.OrderBy(l => l.LabourName)
				.Select(l => new SelectListItem
				{
					Value = l.LabourId.ToString(),
					Text = l.LabourName
				})
				.ToListAsync();

		private async Task<List<SelectListItem>> GetAdvanceTypeOptions() =>
			await _ctx.AdvanceTypes
				.OrderBy(a => a.AdvanceTypeId)
				.Select(a => new SelectListItem
				{
					Value = a.AdvanceTypeId.ToString(),
					Text = a.AdvanceTypeName ?? ""
				})
				.ToListAsync();

		// Manual join — no navigation properties used, avoids CS1061
		private async Task<List<LabourAdvanceListItem>> GetAdvanceList()
		{
			var advances = await _ctx.LabourAdvances.ToListAsync();
			var labours = await _ctx.LabourMasters.ToListAsync();
			var types = await _ctx.AdvanceTypes.ToListAsync();

			// Running total per labour
			var totals = advances
				.GroupBy(a => a.LabourId ?? 0)
				.ToDictionary(g => g.Key, g => g.Sum(a => a.AdvanceAmount ?? 0));

			var result = (from a in advances
						  join l in labours on a.LabourId equals l.LabourId into lj
						  from l in lj.DefaultIfEmpty()
						  join t in types on a.AdvanceTypeId equals t.AdvanceTypeId into tj
						  from t in tj.DefaultIfEmpty()
						  orderby a.AdvanceId descending
						  select new LabourAdvanceListItem
						  {
							  AdvanceId = a.AdvanceId,
							  LabourId = a.LabourId ?? 0,
							  LabourName = l != null ? l.LabourName : "—",
							  AdvanceDate = a.AdvanceDate ?? DateOnly.FromDateTime(DateTime.Today),
							  AdvanceTypeName = t != null ? (t.AdvanceTypeName ?? "—") : "—",
							  AdvanceAmount = a.AdvanceAmount ?? 0,
							  TotalAdvance = totals.GetValueOrDefault(a.LabourId ?? 0, 0)
						  }).ToList();

			return result;
		}

		// ══ INDEX ════════════════════════════════════════════════════
		[HttpGet]
		public async Task<IActionResult> Index(int? editId)
		{
			if (!IsLoggedIn()) return RedirectToAction("Index", "Login");

			var vm = new LabourAdvanceVM
			{
				AdvanceDate = DateOnly.FromDateTime(DateTime.Today),
				LabourOptions = await GetLabourOptions(),
				AdvanceTypeOptions = await GetAdvanceTypeOptions(),
				AdvanceList = await GetAdvanceList()
			};

			if (editId.HasValue)
			{
				var entity = await _ctx.LabourAdvances.FindAsync(editId.Value);
				if (entity != null)
				{
					vm.AdvanceId = entity.AdvanceId;
					vm.LabourId = entity.LabourId;
					vm.AdvanceDate = entity.AdvanceDate;
					vm.AdvanceTypeId = entity.AdvanceTypeId;
					vm.AdvanceAmount = entity.AdvanceAmount;
				}
			}

			return View(vm);
		}

		// ══ SAVE — Insert or Update ══════════════════════════════════
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Save(LabourAdvanceVM model)
		{
			if (!IsLoggedIn()) return RedirectToAction("Index", "Login");

			model.LabourOptions = await GetLabourOptions();
			model.AdvanceTypeOptions = await GetAdvanceTypeOptions();
			model.AdvanceList = await GetAdvanceList();

			if (!ModelState.IsValid)
				return View("Index", model);

			if (model.AdvanceId == 0)
			{
				var entity = new LabourAdvance
				{
					LabourId = model.LabourId,
					AdvanceDate = model.AdvanceDate,
					AdvanceTypeId = model.AdvanceTypeId,
					AdvanceAmount = model.AdvanceAmount
				};
				_ctx.LabourAdvances.Add(entity);
				await _ctx.SaveChangesAsync();
				TempData["Success"] = "Advance recorded successfully.";
			}
			else
			{
				var entity = await _ctx.LabourAdvances.FindAsync(model.AdvanceId);
				if (entity == null)
				{
					TempData["Error"] = "Advance record not found.";
					return RedirectToAction(nameof(Index));
				}
				entity.LabourId = model.LabourId;
				entity.AdvanceDate = model.AdvanceDate;
				entity.AdvanceTypeId = model.AdvanceTypeId;
				entity.AdvanceAmount = model.AdvanceAmount;
				await _ctx.SaveChangesAsync();
				TempData["Success"] = "Advance updated successfully.";
			}

			return RedirectToAction(nameof(Index));
		}

		// ══ DELETE ═══════════════════════════════════════════════════
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Delete(int id)
		{
			if (!IsLoggedIn()) return RedirectToAction("Index", "Login");

			var entity = await _ctx.LabourAdvances.FindAsync(id);
			if (entity != null)
			{
				_ctx.LabourAdvances.Remove(entity);
				await _ctx.SaveChangesAsync();
				TempData["Success"] = "Advance record deleted.";
			}
			else
			{
				TempData["Error"] = "Record not found.";
			}

			return RedirectToAction(nameof(Index));
		}

		// ══ GET TOTAL ADVANCE FOR LABOUR (AJAX) ══════════════════════
		[HttpGet]
		public async Task<IActionResult> GetLabourTotal(int labourId)
		{
			if (!IsLoggedIn()) return Json(new { success = false });

			var totaladvance = await _ctx.LabourAdvances
				.Where(a => a.LabourId == labourId)
				.SumAsync(a => (decimal?)(a.AdvanceAmount ?? 0)) ?? 0;

			var labourName = await _ctx.LabourMasters
				.Where(l => l.LabourId == labourId)
				.Select(l => l.LabourName)
				.FirstOrDefaultAsync() ?? "";

			// Sum all deductions already applied
			var totalDeducted = await _ctx.AdvanceDeductions
							.Where(d => d.LabourId == labourId)
							.SumAsync(d => (decimal?)(d.Amount ?? 0)) ?? 0;

			var total= Math.Max(0, totaladvance - totalDeducted);
			return Json(new { success = true, total, labourName });
		}
	}
}
