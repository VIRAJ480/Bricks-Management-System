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

		private async Task<List<SalaryPaymentListItem>> GetSalaryList()
		{
			// Materialise first, then project in memory — avoids all EF cast errors
			var raw = await _ctx.SalaryPayments
				.Include(s => s.Labour)
				.Include(s => s.Status)
				.OrderByDescending(s => s.SalaryDate)
				.ThenByDescending(s => s.SalaryId)
				.ToListAsync();

			return raw.Select(s => new SalaryPaymentListItem
			{
				SalaryId = s.SalaryId,
				LabourName = s.Labour?.LabourName ?? "—",
				SalaryDate = s.SalaryDate.HasValue
							? DateOnly.FromDateTime(s.SalaryDate.Value.ToDateTime(TimeOnly.MinValue))
							: DateOnly.FromDateTime(DateTime.Today),
				DailyWage = s.DailyWage ?? 0,
				WorkingDays = s.WorkingDays ?? 0,   // int? ?? int → int ✓
				TotalSalary = s.TotalSalary ?? 0,
				TotalExpense = s.TotalExpense ?? 0,
				FinalSalary = s.FinalSalary ?? 0,
				PaidAmount = s.PaidAmount ?? 0,
				StatusName = s.Status?.StatusName ?? "—"
			}).ToList();
		}

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
					vm.SalaryDate = entity.SalaryDate.HasValue
										? DateOnly.FromDateTime(entity.SalaryDate.Value.ToDateTime(TimeOnly.MinValue))
										: DateOnly.FromDateTime(DateTime.Today);
					vm.DailyWage = entity.DailyWage;
					vm.WorkingDays = entity.WorkingDays;
					vm.TotalSalary = entity.TotalSalary;
					vm.TotalExpense = entity.TotalExpense;
					vm.FinalSalary = entity.FinalSalary;
					vm.PaidAmount = entity.PaidAmount;
					vm.StatusId = entity.StatusId;
				}
			}

			return View(vm);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Save(SalaryPaymentVM model)
		{
			if (!IsLoggedIn()) return RedirectToAction("Index", "Login");

			decimal dailyWage = model.DailyWage ?? 0;
			int workingDays = model.WorkingDays ?? 0;          // ← int, not decimal
			decimal totalSalary = dailyWage * workingDays;      // decimal * int = decimal ✓
			decimal totalExpense = model.TotalExpense ?? 0;
			decimal finalSalary = totalSalary - totalExpense;
			decimal paid = model.PaidAmount ?? 0;

			model.TotalSalary = totalSalary;
			model.FinalSalary = finalSalary;

			if (paid > finalSalary)
				ModelState.AddModelError("PaidAmount", "दिलेली रक्कम अंतिम पगारापेक्षा जास्त असू शकत नाही ");

			await PopulateDropdowns(model);
			model.SalaryList = await GetSalaryList();

			if (!ModelState.IsValid)
				return View("Index", model);

			DateOnly salaryDate = model.SalaryDate!.Value;

			if (model.SalaryId == 0)
			{
				var entity = new SalaryPayment
				{
					LabourId = model.LabourId,
					SalaryDate = salaryDate,
					DailyWage = dailyWage,
					WorkingDays = workingDays,
					TotalSalary = totalSalary,
					TotalExpense = totalExpense,
					FinalSalary = finalSalary,
					PaidAmount = paid,
					StatusId = model.StatusId
				};
				_ctx.SalaryPayments.Add(entity);
				await _ctx.SaveChangesAsync();
				TempData["Success"] = $"पगार यशस्वीरित्या नोंद झाला. अंतिम रक्कम: ₹{finalSalary:N2}";
			}
			else
			{
				var entity = await _ctx.SalaryPayments.FindAsync(model.SalaryId);
				if (entity == null)
				{
					TempData["Error"] = "पगाराची नोंद सापडली नाही.";
					return RedirectToAction(nameof(Index));
				}

				entity.LabourId = model.LabourId;
				entity.SalaryDate = salaryDate;
				entity.DailyWage = dailyWage;
				entity.WorkingDays = workingDays;
				entity.TotalSalary = totalSalary;
				entity.TotalExpense = totalExpense;
				entity.FinalSalary = finalSalary;
				entity.PaidAmount = paid;
				entity.StatusId = model.StatusId;

				await _ctx.SaveChangesAsync();
				TempData["Success"] = $"पगार यशस्वीरित्या अपडेट झाला. अंतिम रक्कम: ₹{finalSalary:N2}";
			}

			return RedirectToAction(nameof(Index));
		}

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
				TempData["Success"] = "पगाराची नोंद डिलीट झाली.";
			}
			else
			{
				TempData["Error"] = "पगाराची नोंद सापडली नाही.";
			}

			return RedirectToAction(nameof(Index));
		}
	}
}