using Brick_Manufacturing_Management_System.DBContext;
using Brick_Manufacturing_Management_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Brick_Manufacturing_Management_System.Controllers
{
	public class AdvanceDeductionController : Controller
	{
		private readonly BrickErpdbContext _ctx;

		public AdvanceDeductionController(BrickErpdbContext context)
		{
			_ctx = context;
		}

		// ── Auth guard ────────────────────────────────────────────────────────
		private bool IsLoggedIn() =>
			HttpContext.Session.GetString("Username") != null;

		// ── Populate labour dropdown ──────────────────────────────────────────
		private async Task PopulateDropdowns(AdvanceDeductionVM vm)
		{
			try
			{
				vm.LabourOptions = await _ctx.LabourMasters
					.OrderBy(l => l.LabourName)
					.Select(l => new SelectListItem
					{
						Value = l.LabourId.ToString(),
						Text = l.LabourName
					})
					.ToListAsync();
			}
			catch (Exception ex)
			{
				TempData["Error"] = $"Failed to load labour list: {ex.Message}";
				vm.LabourOptions = new List<SelectListItem>();
			}
		}

		// ── Build flat deduction list for table ───────────────────────────────
		private async Task<List<AdvanceDeductionListItem>> GetDeductionList()
		{
			try
			{
				return await _ctx.AdvanceDeductions
					.Include(d => d.Labour)
					.OrderByDescending(d => d.DeductionDate)
					.ThenByDescending(d => d.DeductionId)
					.Select(d => new AdvanceDeductionListItem
					{
						DeductionId = d.DeductionId,
						DeductionDate = d.DeductionDate ?? DateOnly.FromDateTime(DateTime.Today),
						LabourName = d.Labour != null ? d.Labour.LabourName : "—",
						Amount = d.Amount ?? 0,
						Reason = d.Reason ?? string.Empty
					})
					.ToListAsync();
			}
			catch (Exception ex)
			{
				TempData["Error"] = $"Failed to load deduction records: {ex.Message}";
				return new List<AdvanceDeductionListItem>();
			}
		}

		// ── Get total advance taken by a labour (from LabourAdvance table) ────
		private async Task<decimal> GetLabourAdvance(int labourId)
		{
			try
			{
				// Sum all advances taken by this labour
				var totalAdvance = await _ctx.LabourAdvances
					.Where(a => a.LabourId == labourId)
					.SumAsync(a => (decimal?)(a.AdvanceAmount ?? 0)) ?? 0;

				// Sum all deductions already applied
				var totalDeducted = await _ctx.AdvanceDeductions
					.Where(d => d.LabourId == labourId)
					.SumAsync(d => (decimal?)(d.Amount ?? 0)) ?? 0;

				return Math.Max(0, totalAdvance - totalDeducted);
			}
			catch
			{
				return 0;
			}
		}

		// ══ INDEX ─────────────────────────────────────────────────────────────
		[HttpGet]
		public async Task<IActionResult> Index(int? editId)
		{
			if (!IsLoggedIn()) return RedirectToAction("Index", "Login");

			var vm = new AdvanceDeductionVM
			{
				DeductionDate = DateOnly.FromDateTime(DateTime.Today)
			};

			await PopulateDropdowns(vm);
			vm.DeductionList = await GetDeductionList();

			if (editId.HasValue)
			{
				try
				{
					var entity = await _ctx.AdvanceDeductions
						.Include(d => d.Labour)
						.FirstOrDefaultAsync(d => d.DeductionId == editId.Value);

					if (entity != null)
					{
						vm.DeductionId = entity.DeductionId;
						vm.DeductionDate = entity.DeductionDate;
						vm.LabourId = entity.LabourId;
						vm.Amount = entity.Amount;
						vm.Reason = entity.Reason;

						if (entity.LabourId.HasValue)
							vm.CurrentAdvance = await GetLabourAdvance(entity.LabourId.Value);
					}
					else
					{
						TempData["Error"] = "Deduction record not found.";
					}
				}
				catch (Exception ex)
				{
					TempData["Error"] = $"Failed to load record for editing: {ex.Message}";
				}
			}

			return View(vm);
		}

		// ── AJAX: get advance balance for selected labour ──────────────────────
		[HttpGet]
		public async Task<IActionResult> GetAdvanceInfo(int labourId)
		{
			try
			{
				if (labourId <= 0)
					return Json(new { success = false, message = "Invalid labour ID." });

				var labour = await _ctx.LabourMasters
					.Where(l => l.LabourId == labourId)
					.Select(l => new { l.LabourName })
					.FirstOrDefaultAsync();

				if (labour == null)
					return Json(new { success = false, message = "Labour not found." });

				var totalAdvance = await _ctx.LabourAdvances
					.Where(a => a.LabourId == labourId)
					.SumAsync(a => (decimal?)(a.AdvanceAmount ?? 0)) ?? 0;

				var totalDeducted = await _ctx.AdvanceDeductions
					.Where(d => d.LabourId == labourId)
					.SumAsync(d => (decimal?)(d.Amount ?? 0)) ?? 0;

				var balance = Math.Max(0, totalAdvance - totalDeducted);

				return Json(new
				{
					success = true,
					totalAdvance = totalAdvance,
					totalDeducted = totalDeducted,
					balance = balance
				});
			}
			catch (Exception ex)
			{
				return Json(new { success = false, message = ex.Message });
			}
		}

		// ══ SAVE — Insert or Update ───────────────────────────────────────────
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Save(AdvanceDeductionVM model)
		{
			if (!IsLoggedIn()) return RedirectToAction("Index", "Login");

			// Rebuild for re-render on error
			await PopulateDropdowns(model);
			model.DeductionList = await GetDeductionList();

			if (model.LabourId.HasValue)
				model.CurrentAdvance = await GetLabourAdvance(model.LabourId.Value);

			if (!ModelState.IsValid)
				return View("Index", model);

			// Validate deduction does not exceed available advance balance
			if (model.LabourId.HasValue && model.Amount.HasValue)
			{
				// For update, exclude current record's amount from balance calc
				decimal currentBalance = model.CurrentAdvance;
				if (model.DeductionId > 0)
				{
					var existingAmount = await _ctx.AdvanceDeductions
						.Where(d => d.DeductionId == model.DeductionId)
						.Select(d => d.Amount ?? 0)
						.FirstOrDefaultAsync();
					currentBalance += existingAmount; // add back original amount
				}

				if (model.Amount.Value > currentBalance)
				{
					ModelState.AddModelError("Amount",
						$"Deduction (₹{model.Amount.Value:N2}) cannot exceed available advance balance (₹{currentBalance:N2}).");
					return View("Index", model);
				}
			}

			try
			{
				if (model.DeductionId == 0)
				{
					// ── INSERT ──
					var entity = new AdvanceDeduction
					{
						DeductionDate = model.DeductionDate!.Value,
						LabourId = model.LabourId,
						Amount = model.Amount,
						Reason = model.Reason?.Trim()
					};
					_ctx.AdvanceDeductions.Add(entity);
					await _ctx.SaveChangesAsync();
					TempData["Success"] = $"Deduction recorded successfully. Amount: ₹{model.Amount:N2}";
				}
				else
				{
					// ── UPDATE ──
					var entity = await _ctx.AdvanceDeductions.FindAsync(model.DeductionId);
					if (entity == null)
					{
						TempData["Error"] = "Deduction record not found. It may have been deleted.";
						return RedirectToAction(nameof(Index));
					}

					entity.DeductionDate = model.DeductionDate!.Value;
					entity.LabourId = model.LabourId;
					entity.Amount = model.Amount;
					entity.Reason = model.Reason?.Trim();
					await _ctx.SaveChangesAsync();
					TempData["Success"] = $"Deduction updated successfully. Amount: ₹{model.Amount:N2}";
				}
			}
			catch (DbUpdateException dbEx)
			{
				ModelState.AddModelError("", $"Database error: {dbEx.InnerException?.Message ?? dbEx.Message}");
				return View("Index", model);
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", $"Unexpected error: {ex.Message}");
				return View("Index", model);
			}

			return RedirectToAction(nameof(Index));
		}

		// ══ DELETE ───────────────────────────────────────────────────────────
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Delete(int id)
		{
			if (!IsLoggedIn()) return RedirectToAction("Index", "Login");

			if (id <= 0)
			{
				TempData["Error"] = "Invalid record ID.";
				return RedirectToAction(nameof(Index));
			}

			try
			{
				var entity = await _ctx.AdvanceDeductions.FindAsync(id);
				if (entity == null)
				{
					TempData["Error"] = "Deduction record not found. It may have already been deleted.";
					return RedirectToAction(nameof(Index));
				}

				_ctx.AdvanceDeductions.Remove(entity);
				await _ctx.SaveChangesAsync();
				TempData["Success"] = "Deduction record deleted successfully.";
			}
			catch (DbUpdateException dbEx)
			{
				TempData["Error"] = $"Cannot delete: {dbEx.InnerException?.Message ?? dbEx.Message}";
			}
			catch (Exception ex)
			{
				TempData["Error"] = $"Unexpected error while deleting: {ex.Message}";
			}

			return RedirectToAction(nameof(Index));
		}
	}
}
