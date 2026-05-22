using Brick_Manufacturing_Management_System.DBContext;
using Brick_Manufacturing_Management_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Brick_Manufacturing_Management_System.Controllers
{
	public class LabourWorkController : Controller
	{
		private readonly BrickErpdbContext _ctx;

		public LabourWorkController(BrickErpdbContext context)
		{
			_ctx = context;
		}

		// ── Auth guard ────────────────────────────────────────────────────────
		private bool IsLoggedIn() =>
			HttpContext.Session.GetString("Username") != null;

		// ── Populate labour dropdown ──────────────────────────────────────────
		private async Task PopulateDropdowns(LabourWorkVM vm)
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

		// ── Build flat work list for table ────────────────────────────────────
		private async Task<List<LabourWorkListItem>> GetWorkList()
		{
			try
			{
				return await _ctx.LabourWorks
					.Include(w => w.Labour)
					.OrderByDescending(w => w.WorkDate)
					.ThenByDescending(w => w.WorkId)
					.Select(w => new LabourWorkListItem
					{
						WorkId = w.WorkId,
						WorkDate = w.WorkDate ?? DateOnly.FromDateTime(DateTime.Today),
						LabourName = w.Labour != null ? w.Labour.LabourName : "—",
						DailyWage = w.DailyWage ?? 0,
						DaysWorked = w.DaysWorked ?? 0,
						TotalSalary = w.TotalSalary ?? 0
					})
					.ToListAsync();
			}
			catch (Exception ex)
			{
				TempData["Error"] = $"Failed to load work records: {ex.Message}";
				return new List<LabourWorkListItem>();
			}
		}

		// ══ INDEX ─────────────────────────────────────────────────────────────
		[HttpGet]
		public async Task<IActionResult> Index(int? editId)
		{
			if (!IsLoggedIn()) return RedirectToAction("Index", "Login");

			var vm = new LabourWorkVM
			{
				WorkDate = DateOnly.FromDateTime(DateTime.Today)
			};

			await PopulateDropdowns(vm);
			vm.WorkList = await GetWorkList();

			if (editId.HasValue)
			{
				try
				{
					var entity = await _ctx.LabourWorks
						.Include(w => w.Labour)
						.FirstOrDefaultAsync(w => w.WorkId == editId.Value);

					if (entity != null)
					{
						vm.WorkId = entity.WorkId;
						vm.WorkDate = entity.WorkDate;
						vm.LabourId = entity.LabourId;
						vm.DailyWage = entity.DailyWage;
						vm.DaysWorked = entity.DaysWorked;
						vm.TotalSalary = entity.TotalSalary;
					}
					else
					{
						TempData["Error"] = "Work record not found.";
					}
				}
				catch (Exception ex)
				{
					TempData["Error"] = $"Failed to load record for editing: {ex.Message}";
				}
			}

			return View(vm);
		}

		// ── GET DAILY WAGE for selected labour (AJAX) ─────────────────────────
		[HttpGet]
		public async Task<IActionResult> GetLabourWage(int labourId)
		{
			try
			{
				var labour = await _ctx.LabourMasters
					.Where(l => l.LabourId == labourId)
					.Select(l => new { l.DailyWage })
					.FirstOrDefaultAsync();

				if (labour == null)
					return Json(new { success = false, message = "Labour not found." });

				return Json(new { success = true, dailyWage = labour.DailyWage ?? 0 });
			}
			catch (Exception ex)
			{
				return Json(new { success = false, message = ex.Message });
			}
		}

		// ══ SAVE — Insert or Update ───────────────────────────────────────────
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Save(LabourWorkVM model)
		{
			if (!IsLoggedIn()) return RedirectToAction("Index", "Login");

			// Auto-calculate total before validation
			decimal wage = model.DailyWage ?? 0;
			int days = model.DaysWorked ?? 0;
			decimal totalSal = wage * days;
			model.TotalSalary = totalSal;

			// Rebuild for re-render on error
			await PopulateDropdowns(model);
			model.WorkList = await GetWorkList();

			if (!ModelState.IsValid)
				return View("Index", model);

			try
			{
				if (model.WorkId == 0)
				{
					// ── INSERT ──
					var entity = new LabourWork
					{
						WorkDate = model.WorkDate!.Value,
						LabourId = model.LabourId,
						DailyWage = wage,
						DaysWorked = days,
						TotalSalary = totalSal
					};
					_ctx.LabourWorks.Add(entity);
					await _ctx.SaveChangesAsync();
					TempData["Success"] = $"Work entry saved. Total Salary: ₹{totalSal:N2}";
				}
				else
				{
					// ── UPDATE ──
					var entity = await _ctx.LabourWorks.FindAsync(model.WorkId);
					if (entity == null)
					{
						TempData["Error"] = "Work record not found. It may have been deleted.";
						return RedirectToAction(nameof(Index));
					}

					entity.WorkDate = model.WorkDate!.Value;
					entity.LabourId = model.LabourId;
					entity.DailyWage = wage;
					entity.DaysWorked = days;
					entity.TotalSalary = totalSal;
					await _ctx.SaveChangesAsync();
					TempData["Success"] = $"Work entry updated. Total Salary: ₹{totalSal:N2}";
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
				var entity = await _ctx.LabourWorks.FindAsync(id);
				if (entity == null)
				{
					TempData["Error"] = "Work record not found. It may have already been deleted.";
					return RedirectToAction(nameof(Index));
				}

				_ctx.LabourWorks.Remove(entity);
				await _ctx.SaveChangesAsync();
				TempData["Success"] = "Work record deleted successfully.";
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
