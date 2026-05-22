using Brick_Manufacturing_Management_System.DBContext;
using Brick_Manufacturing_Management_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Brick_Manufacturing_Management_System.Controllers
{
	public class VendorPaymentController : Controller
	{
		private readonly BrickErpdbContext _ctx;

		public VendorPaymentController(BrickErpdbContext context)
		{
			_ctx = context;
		}

		private bool IsLoggedIn() =>
			HttpContext.Session.GetString("Username") != null;

		// ── Helper: build all dropdowns ─────────────────────────────────────
		private async Task PopulateDropdowns(VendorPaymentVM vm)
		{
			vm.VendorOptions = await _ctx.VendorMasters
				.OrderBy(v => v.VendorName)
				.Select(v => new SelectListItem
				{
					Value = v.VendorId.ToString(),
					Text = v.VendorName ?? "—"
				})
				.ToListAsync();

			vm.PaymentModeOptions = await _ctx.PaymentModes
				.OrderBy(p => p.PaymentModeName)
				.Select(p => new SelectListItem
				{
					Value = p.PaymentModeId.ToString(),
					Text = p.PaymentModeName ?? "—"
				})
				.ToListAsync();

			vm.StatusOptions = await _ctx.PaymentStatuses
				.OrderBy(s => s.StatusName)
				.Select(s => new SelectListItem
				{
					Value = s.StatusId.ToString(),
					Text = s.StatusName ?? "—"
				})
				.ToListAsync();
		}

		// ── Helper: build flat payment list ────────────────────────────────
		private async Task<List<VendorPaymentListItem>> GetPaymentList()
		{
			return await _ctx.VendorPayments
				.OrderByDescending(p => p.PaymentDate)
				.ThenByDescending(p => p.PaymentId)
				.Select(p => new VendorPaymentListItem
				{
					PaymentId = p.PaymentId,
					PaymentDate = p.PaymentDate.HasValue ? p.PaymentDate.Value : DateOnly.FromDateTime(DateTime.Today),
					VendorName = p.Vendor != null ? p.Vendor.VendorName ?? "—" : "—",
					Amount = p.Amount,
					PaymentMode = p.PaymentMode != null ? p.PaymentMode.PaymentModeName ?? "—" : "—",
					StatusName = p.Status != null ? p.Status.StatusName ?? "—" : "—"
				})
				.ToListAsync();
		}

		// ══ INDEX ────────────────────────────────────────────────────────────
		[HttpGet]
		public async Task<IActionResult> Index(int? editId)
		{
			if (!IsLoggedIn()) return RedirectToAction("Index", "Login");

			var vm = new VendorPaymentVM
			{
				PaymentDate = DateOnly.FromDateTime(DateTime.Today)
			};

			await PopulateDropdowns(vm);
			vm.PaymentList = await GetPaymentList();

			if (editId.HasValue)
			{
				var entity = await _ctx.VendorPayments.FindAsync(editId.Value);
				if (entity != null)
				{
					vm.PaymentId = entity.PaymentId;
					vm.PaymentDate = entity.PaymentDate;
					vm.VendorId = entity.VendorId;
					vm.Amount = entity.Amount;
					vm.PaymentModeId = entity.PaymentModeId;
					vm.StatusId = entity.StatusId;
				}
			}

			return View(vm);
		}

		// ══ SAVE — Insert or Update ──────────────────────────────────────────
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Save(VendorPaymentVM model)
		{
			if (!IsLoggedIn()) return RedirectToAction("Index", "Login");

			await PopulateDropdowns(model);
			model.PaymentList = await GetPaymentList();

			if (!ModelState.IsValid)
				return View("Index", model);

			if (!model.PaymentDate.HasValue)
			{
				ModelState.AddModelError("PaymentDate", "Payment date is required.");
				return View("Index", model);
			}

			DateOnly paymentDate = model.PaymentDate.Value;

			if (model.PaymentId == 0)
			{
				// INSERT
				var entity = new VendorPayment
				{
					PaymentDate = paymentDate,
					VendorId = model.VendorId!.Value,
					Amount = model.Amount!.Value,
					PaymentModeId = model.PaymentModeId!.Value,
					StatusId = model.StatusId!.Value
				};
				_ctx.VendorPayments.Add(entity);
				await _ctx.SaveChangesAsync();
				TempData["Success"] = $"Vendor payment of ₹{model.Amount:N2} recorded successfully!";
			}
			else
			{
				// UPDATE
				var entity = await _ctx.VendorPayments.FindAsync(model.PaymentId);
				if (entity == null)
				{
					TempData["Error"] = "Payment record not found.";
					return RedirectToAction(nameof(Index));
				}

				entity.PaymentDate = paymentDate;
				entity.VendorId = model.VendorId!.Value;
				entity.Amount = model.Amount!.Value;
				entity.PaymentModeId = model.PaymentModeId!.Value;
				entity.StatusId = model.StatusId!.Value;
				await _ctx.SaveChangesAsync();
				TempData["Success"] = "Vendor payment updated successfully!";
			}

			return RedirectToAction(nameof(Index));
		}

		// ══ DELETE ───────────────────────────────────────────────────────────
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Delete(int id)
		{
			if (!IsLoggedIn()) return RedirectToAction("Index", "Login");

			var entity = await _ctx.VendorPayments.FindAsync(id);
			if (entity != null)
			{
				_ctx.VendorPayments.Remove(entity);
				await _ctx.SaveChangesAsync();
				TempData["Success"] = "Vendor payment record deleted.";
			}
			else
			{
				TempData["Error"] = "Vendor payment record not found.";
			}

			return RedirectToAction(nameof(Index));
		}
	}
}
