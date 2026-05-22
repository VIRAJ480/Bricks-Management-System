using Brick_Manufacturing_Management_System.DBContext;
using Brick_Manufacturing_Management_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Brick_Manufacturing_Management_System.Controllers
{
	public class LabourExpenseController : Controller
	{
		private readonly BrickErpdbContext _ctx;

		public LabourExpenseController(BrickErpdbContext context)
		{
			_ctx = context;
		}

		// ── Auth guard ────────────────────────────────────────────────────────
		private bool IsLoggedIn() =>
			HttpContext.Session.GetString("Username") != null;

		// ── Populate labour dropdown ──────────────────────────────────────────
		private async Task PopulateDropdowns(LabourExpenseVM vm)
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

		// ── Build flat expense list for table ─────────────────────────────────
		private async Task<List<LabourExpenseListItem>> GetExpenseList()
		{
			try
			{
				return await _ctx.LabourExpenses
					.Include(e => e.Labour)
					.OrderByDescending(e => e.ExpenseDate)
					.ThenByDescending(e => e.ExpenseId)
					.Select(e => new LabourExpenseListItem
					{
						ExpenseId = e.ExpenseId,
						ExpenseDate = e.ExpenseDate ?? DateOnly.FromDateTime(DateTime.Today),
						LabourName = e.Labour != null ? e.Labour.LabourName : "—",
						Amount = e.Amount ?? 0,
						Reason = e.Reason ?? string.Empty
					})
					.ToListAsync();
			}
			catch (Exception ex)
			{
				TempData["Error"] = $"Failed to load expense records: {ex.Message}";
				return new List<LabourExpenseListItem>();
			}
		}

		// ══ INDEX ─────────────────────────────────────────────────────────────
		[HttpGet]
		public async Task<IActionResult> Index(int? editId)
		{
			if (!IsLoggedIn()) return RedirectToAction("Index", "Login");

			var vm = new LabourExpenseVM
			{
				ExpenseDate = DateOnly.FromDateTime(DateTime.Today)
			};

			await PopulateDropdowns(vm);
			vm.ExpenseList = await GetExpenseList();

			if (editId.HasValue)
			{
				try
				{
					var entity = await _ctx.LabourExpenses
						.Include(e => e.Labour)
						.FirstOrDefaultAsync(e => e.ExpenseId == editId.Value);

					if (entity != null)
					{
						vm.ExpenseId = entity.ExpenseId;
						vm.ExpenseDate = entity.ExpenseDate;
						vm.LabourId = entity.LabourId;
						vm.Amount = entity.Amount;
						vm.Reason = entity.Reason;
					}
					else
					{
						TempData["Error"] = "Expense record not found.";
					}
				}
				catch (Exception ex)
				{
					TempData["Error"] = $"Failed to load record for editing: {ex.Message}";
				}
			}

			return View(vm);
		}

		// ══ SAVE — Insert or Update ───────────────────────────────────────────
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Save(LabourExpenseVM model)
		{
			if (!IsLoggedIn()) return RedirectToAction("Index", "Login");

			// Rebuild dropdowns & list for re-render on error
			await PopulateDropdowns(model);
			model.ExpenseList = await GetExpenseList();

			if (!ModelState.IsValid)
				return View("Index", model);

			try
			{
				if (model.ExpenseId == 0)
				{
					// ── INSERT ──
					var entity = new LabourExpense
					{
						ExpenseDate = model.ExpenseDate!.Value,
						LabourId = model.LabourId,
						Amount = model.Amount,
						Reason = model.Reason?.Trim()
					};
					_ctx.LabourExpenses.Add(entity);
					await _ctx.SaveChangesAsync();
					TempData["Success"] = $"Expense recorded successfully. Amount: ₹{model.Amount:N2}";
				}
				else
				{
					// ── UPDATE ──
					var entity = await _ctx.LabourExpenses.FindAsync(model.ExpenseId);
					if (entity == null)
					{
						TempData["Error"] = "Expense record not found. It may have been deleted.";
						return RedirectToAction(nameof(Index));
					}

					entity.ExpenseDate = model.ExpenseDate!.Value;
					entity.LabourId = model.LabourId;
					entity.Amount = model.Amount;
					entity.Reason = model.Reason?.Trim();
					await _ctx.SaveChangesAsync();
					TempData["Success"] = $"Expense updated successfully. Amount: ₹{model.Amount:N2}";
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
				var entity = await _ctx.LabourExpenses.FindAsync(id);
				if (entity == null)
				{
					TempData["Error"] = "Expense record not found. It may have already been deleted.";
					return RedirectToAction(nameof(Index));
				}

				_ctx.LabourExpenses.Remove(entity);
				await _ctx.SaveChangesAsync();
				TempData["Success"] = "Expense record deleted successfully.";
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
