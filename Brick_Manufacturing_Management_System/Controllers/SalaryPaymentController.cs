using Brick_Manufacturing_Management_System.DBContext;
using Brick_Manufacturing_Management_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Brick_Manufacturing_Management_System.Controllers
{
	public class SalaryPaymentController : Controller
	{
		private readonly BrickErpdbContext _ctx;

		public SalaryPaymentController(BrickErpdbContext context)
		{
			_ctx = context;
		}

		private bool IsLoggedIn() =>
			HttpContext.Session.GetString("Username") != null;

		// ── Helper: populate dropdowns ────────────────────────────────────────
		private async Task PopulateDropdowns(SalaryPaymentVM vm)
		{
			vm.LabourOptions = await _ctx.LabourMasters
				.OrderBy(l => l.LabourName)
				.Select(l => new SelectListItem
				{
					Value = l.LabourId.ToString(),
					Text = l.LabourName
				})
				.ToListAsync();

			vm.StatusOptions = await _ctx.PaymentStatuses
				.OrderBy(s => s.StatusId)
				.Select(s => new SelectListItem
				{
					Value = s.StatusId.ToString(),
					Text = s.StatusName
				})
				.ToListAsync();
		}

		// ── Helper: flat list for table ───────────────────────────────────────
		private async Task<List<SalaryPaymentListItem>> GetSalaryList()
		{
			return await _ctx.SalaryPayments
				.Include(s => s.Labour)
				.Include(s => s.Status)
				.OrderByDescending(s => s.SalaryDate)
				.ThenByDescending(s => s.SalaryId)
				.Select(s => new SalaryPaymentListItem
				{
					SalaryId = s.SalaryId,
					LabourName = s.Labour != null ? s.Labour.LabourName : "—",
					SalaryDate = s.SalaryDate != null
									? DateOnly.FromDateTime(s.SalaryDate.Value.ToDateTime(TimeOnly.MinValue))
									: DateOnly.FromDateTime(DateTime.Today),
					TotalSalary = s.TotalSalary ?? 0,
					TotalExpense = s.TotalExpense ?? 0,
					FinalSalary = s.FinalSalary ?? 0,
					PaidAmount = s.PaidAmount ?? 0,
					StatusName = s.Status != null ? s.Status.StatusName ?? "—" : "—"
				})
				.ToListAsync();
		}

		// ── Helper: calculate labour aggregates from LabourWork & LabourExpense
		private async Task<(decimal totalSalary, decimal totalExpense)> GetLabourAggregates(int labourId)
		{
			decimal totalSalary = await _ctx.LabourWorks
				.Where(w => w.LabourId == labourId)
				.SumAsync(w => (decimal?)(w.DailyWage * w.DaysWorked)) ?? 0;

			decimal totalExpense = await _ctx.LabourExpenses
				.Where(e => e.LabourId == labourId)
				.SumAsync(e => (decimal?)e.Amount) ?? 0;

			return (totalSalary, totalExpense);
		}

		// ══ INDEX ─────────────────────────────────────────────────────────────
		[HttpGet]
		public async Task<IActionResult> Index(int? editId)
		{
			if (!IsLoggedIn()) return RedirectToAction("Index", "Login");

			var vm = new SalaryPaymentVM
			{
				SalaryDate = DateOnly.FromDateTime(DateTime.Today)
			};

			await PopulateDropdowns(vm);
			vm.SalaryList = await GetSalaryList();

			if (editId.HasValue)
			{
				var entity = await _ctx.SalaryPayments.FindAsync(editId.Value);
				if (entity != null)
				{
					vm.SalaryId = entity.SalaryId;
					vm.LabourId = entity.LabourId;
					vm.SalaryDate = entity.SalaryDate != null
										 ? DateOnly.FromDateTime(entity.SalaryDate.Value.ToDateTime(TimeOnly.MinValue))
										 : DateOnly.FromDateTime(DateTime.Today);
					vm.TotalSalary = entity.TotalSalary;
					vm.TotalExpense = entity.TotalExpense;
					vm.FinalSalary = entity.FinalSalary;
					vm.PaidAmount = entity.PaidAmount;
					vm.StatusId = entity.StatusId;
				}
			}

			return View(vm);
		}

		// ── AJAX: get labour aggregates when labour dropdown changes ──────────
		[HttpGet]
		public async Task<IActionResult> GetLabourData(int labourId)
		{
			if (!IsLoggedIn()) return Unauthorized();

			var (totalSalary, totalExpense) = await GetLabourAggregates(labourId);
			decimal finalSalary = totalSalary - totalExpense;

			return Json(new
			{
				totalSalary = totalSalary,
				totalExpense = totalExpense,
				finalSalary = finalSalary
			});
		}

		// ══ SAVE — Insert or Update ───────────────────────────────────────────
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Save(SalaryPaymentVM model)
		{
			if (!IsLoggedIn()) return RedirectToAction("Index", "Login");

			// Recalculate server-side
			decimal totalSalary = model.TotalSalary ?? 0;
			decimal totalExpense = model.TotalExpense ?? 0;
			decimal finalSalary = totalSalary - totalExpense;
			decimal paid = model.PaidAmount ?? 0;

			model.FinalSalary = finalSalary;

			// Paid cannot exceed final salary
			if (paid > finalSalary)
			{
				ModelState.AddModelError("PaidAmount", "Paid amount cannot exceed final salary.");
			}

			// Rebuild for re-render on validation failure
			await PopulateDropdowns(model);
			model.SalaryList = await GetSalaryList();

			if (!ModelState.IsValid)
				return View("Index", model);

			DateOnly salaryDate = model.SalaryDate!.Value;

			if (model.SalaryId == 0)
			{
				// INSERT
				var entity = new SalaryPayment
				{
					LabourId = model.LabourId,
					SalaryDate = new DateOnly?(salaryDate),
					TotalSalary = totalSalary,
					TotalExpense = totalExpense,
					FinalSalary = finalSalary,
					PaidAmount = paid,
					StatusId = model.StatusId
				};
				_ctx.SalaryPayments.Add(entity);
				await _ctx.SaveChangesAsync();
				TempData["Success"] = $"Salary recorded. Final: ₹{finalSalary:N2}";
			}
			else
			{
				// UPDATE
				var entity = await _ctx.SalaryPayments.FindAsync(model.SalaryId);
				if (entity == null)
				{
					TempData["Error"] = "Salary record not found.";
					return RedirectToAction(nameof(Index));
				}

				entity.LabourId = model.LabourId;
				entity.SalaryDate = new DateOnly?(salaryDate);
				entity.TotalSalary = totalSalary;
				entity.TotalExpense = totalExpense;
				entity.FinalSalary = finalSalary;
				entity.PaidAmount = paid;
				entity.StatusId = model.StatusId;

				await _ctx.SaveChangesAsync();
				TempData["Success"] = $"Salary updated. Final: ₹{finalSalary:N2}";
			}

			return RedirectToAction(nameof(Index));
		}

		// ══ DELETE ───────────────────────────────────────────────────────────
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Delete(int id)
		{
			if (!IsLoggedIn()) return RedirectToAction("Index", "Login");

			var entity = await _ctx.SalaryPayments.FindAsync(id);
			if (entity != null)
			{
				_ctx.SalaryPayments.Remove(entity);
				await _ctx.SaveChangesAsync();
				TempData["Success"] = "Salary record deleted.";
			}
			else
			{
				TempData["Error"] = "Salary record not found.";
			}

			return RedirectToAction(nameof(Index));
		}
	}
}
