using Brick_Manufacturing_Management_System.DBContext;
using Brick_Manufacturing_Management_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Brick_Manufacturing_Management_System.Controllers
{
	public class LabourMasterController : Controller
	{
		private readonly BrickErpdbContext _ctx;

		public LabourMasterController(BrickErpdbContext context)
		{
			_ctx = context;
		}

		private bool IsLoggedIn() =>
			HttpContext.Session.GetString("Username") != null;

		// ══ INDEX ════════════════════════════════════════════════════════════
		[HttpGet]
		public async Task<IActionResult> Index(int? editId)
		{
			if (!IsLoggedIn()) return RedirectToAction("Index", "Login");

			var list = await _ctx.LabourMasters
				.OrderByDescending(l => l.LabourId)
				.ToListAsync();

			var vm = new LabourMasterVM { LabourList = list };

			if (editId.HasValue)
			{
				var entity = list.FirstOrDefault(l => l.LabourId == editId.Value);
				if (entity != null)
				{
					vm.LabourId = entity.LabourId;
					vm.LabourName = entity.LabourName;
					vm.MobileNumber = entity.MobileNumber;
					vm.Address = entity.Address;
					vm.DailyWage = entity.DailyWage;
					vm.JoiningDate = entity.JoiningDate;   // ✅ DateOnly? = DateOnly?
				}
			}

			return View(vm);
		}

		// ══ SAVE — Insert or Update ══════════════════════════════════════════
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Save(LabourMasterVM model)
		{
			if (!IsLoggedIn()) return RedirectToAction("Index", "Login");

			model.LabourList = await _ctx.LabourMasters
				.OrderByDescending(l => l.LabourId)
				.ToListAsync();

			if (!ModelState.IsValid)
				return View("Index", model);

			// Duplicate name check
			bool dupName = await _ctx.LabourMasters.AnyAsync(l =>
				l.LabourName.ToLower() == model.LabourName.Trim().ToLower() &&
				l.LabourId != model.LabourId);

			if (dupName)
			{
				ModelState.AddModelError("LabourName", "या नावाचा मजूर आधीपासून आहे.");
				return View("Index", model);
			}

			// Duplicate mobile check
			if (!string.IsNullOrWhiteSpace(model.MobileNumber))
			{
				bool dup = await _ctx.LabourMasters.AnyAsync(l =>
					l.MobileNumber == model.MobileNumber.Trim() &&
					l.LabourId != model.LabourId);

				if (dup)
				{
					ModelState.AddModelError("MobileNumber",
                                                     "या मोबाईल नंबरवरचं मजूराचं नाव आधीपासून आहे.");
					return View("Index", model);
				}
			}

			if (model.LabourId == 0)
			{
				// ── INSERT ──────────────────────────────────────────────────
				var entity = new LabourMaster
				{
					LabourName = model.LabourName.Trim(),
					MobileNumber = model.MobileNumber?.Trim(),
					Address = model.Address?.Trim(),
					DailyWage = model.DailyWage,
					JoiningDate = model.JoiningDate   // ✅ DateOnly? = DateOnly?
				};

				_ctx.LabourMasters.Add(entity);
				await _ctx.SaveChangesAsync();
				TempData["Success"] = $"'{entity.LabourName}' मजूर यशस्वीरित्या अॅड झाला.";
			}
			else
			{
				// ── UPDATE ──────────────────────────────────────────────────
				var entity = await _ctx.LabourMasters.FindAsync(model.LabourId);
				if (entity == null)
				{
					TempData["Error"] = "मजुराची नोंद सापडली नाही.";
					return RedirectToAction(nameof(Index));
				}

				entity.LabourName = model.LabourName.Trim();
				entity.MobileNumber = model.MobileNumber?.Trim();
				entity.Address = model.Address?.Trim();
				entity.DailyWage = model.DailyWage;
				entity.JoiningDate = model.JoiningDate;   // ✅ DateOnly? = DateOnly?

				await _ctx.SaveChangesAsync();
				TempData["Success"] = $"'{entity.LabourName}' मजूर यशस्वीरित्या अपडेट झाला.";
			}

			return RedirectToAction(nameof(Index));
		}

		// ══ DELETE ═══════════════════════════════════════════════════════════
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Delete(int id)
		{
			if (!IsLoggedIn()) return RedirectToAction("Index", "Login");

			var entity = await _ctx.LabourMasters.FindAsync(id);
			if (entity == null)
			{
				TempData["Error"] = "Labour record not found.";
				return RedirectToAction(nameof(Index));
			}

			bool hasAdvance   = await _ctx.LabourAdvances.AnyAsync(x => x.LabourId == id);
			bool hasWork      = await _ctx.LabourWorks.AnyAsync(x => x.LabourId == id);
			bool hasExpense   = await _ctx.LabourExpenses.AnyAsync(x => x.LabourId == id);
			bool hasDeduction = await _ctx.AdvanceDeductions.AnyAsync(x => x.LabourId == id);
			bool hasSalary    = await _ctx.SalaryPayments.AnyAsync(x => x.LabourId == id);

			if (hasAdvance || hasWork || hasExpense || hasDeduction || hasSalary)
			{
				var linked = new List<string>();
				if (hasAdvance)   linked.Add("Labour Advance");
				if (hasWork)      linked.Add("Work Entry");
				if (hasExpense)   linked.Add("Labour Expense");
				if (hasDeduction) linked.Add("Advance Deduction");
				if (hasSalary)    linked.Add("Salary Payment");
				TempData["Error"] = $"'{entity.LabourName}' ला डिलीट करता येणार नाही — {string.Join(", ", linked)} मध्ये याचे रेकॉर्ड आधीपासून आहेत. आधी ते रेकॉर्ड डिलीट करा.";
				return RedirectToAction(nameof(Index));
			}

			_ctx.LabourMasters.Remove(entity);
			await _ctx.SaveChangesAsync();
			TempData["Success"] = $"'{entity.LabourName}' मजूर डिलीट झाला.";

			return RedirectToAction(nameof(Index));
		}
	}
}