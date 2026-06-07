using Brick_Manufacturing_Management_System.DBContext;
using Brick_Manufacturing_Management_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Brick_Manufacturing_Management_System.Controllers
{
	public class CustomerPaymentController : Controller
	{
		private readonly BrickErpdbContext _ctx;

		public CustomerPaymentController(BrickErpdbContext context)
		{
			_ctx = context;
		}

		private bool IsLoggedIn() =>
			HttpContext.Session.GetString("Username") != null;

		// ── Helper: build all dropdowns ─────────────────────────────────────
		private async Task PopulateDropdowns(CustomerPaymentVM vm)
		{
			vm.CustomerOptions = await _ctx.CustomerMasters
				.OrderBy(c => c.CustomerName)
				.Select(c => new SelectListItem
				{
					Value = c.CustomerId.ToString(),
					Text = c.CustomerName ?? "—"   // BUG FIX 1: CustomerName can be null
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
		private async Task<List<CustomerPaymentListItem>> GetPaymentList()
		{
			// BUG FIX 2: .Include() + .Select() together causes EF Core warning/error.
			// Use JOIN-style Select directly without Include() — this is the correct pattern.
			return await _ctx.CustomerPayments
				.OrderByDescending(p => p.PaymentDate)
				.ThenByDescending(p => p.PaymentId)
				.Select(p => new CustomerPaymentListItem
				{
					PaymentId = p.PaymentId,
					PaymentDate = p.PaymentDate ?? DateOnly.FromDateTime(DateTime.Today),
					CustomerName = p.Customer != null ? p.Customer.CustomerName ?? "—" : "—",
					Amount = p.Amount ?? 0,
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

			var vm = new CustomerPaymentVM
			{
				PaymentDate = DateOnly.FromDateTime(DateTime.Today)
			};

			await PopulateDropdowns(vm);
			vm.PaymentList = await GetPaymentList();

			if (editId.HasValue)
			{
				var entity = await _ctx.CustomerPayments.FindAsync(editId.Value);
				if (entity != null)
				{
					vm.PaymentId = entity.PaymentId;
					vm.PaymentDate = entity.PaymentDate;
					vm.CustomerId = entity.CustomerId;
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
		public async Task<IActionResult> Save(CustomerPaymentVM model)
		{
			if (!IsLoggedIn()) return RedirectToAction("Index", "Login");

			// Rebuild dropdowns & list BEFORE ModelState check so re-render works
			await PopulateDropdowns(model);
			model.PaymentList = await GetPaymentList();

			if (!ModelState.IsValid)
				return View("Index", model);

			// BUG FIX 3: model.PaymentDate is [Required] so .Value is safe,
			// but guard against null defensively
			if (!model.PaymentDate.HasValue)
			{
				ModelState.AddModelError("PaymentDate", "पेमेंटची तारीख आवश्यक आहे.");
				return View("Index", model);
			}

			DateOnly paymentDate = model.PaymentDate.Value;

			if (model.PaymentId == 0)
			{
				// INSERT
				var entity = new CustomerPayment
				{
					PaymentDate = paymentDate,
					CustomerId = model.CustomerId,
					Amount = model.Amount,
					PaymentModeId = model.PaymentModeId,
					StatusId = model.StatusId
				};
				_ctx.CustomerPayments.Add(entity);
				await _ctx.SaveChangesAsync();
				TempData["Success"] = $"₹{model.Amount:N2} चे पेमेंट यशस्वीरित्या नोंद झालं!";
			}
			else
			{
				// UPDATE
				var entity = await _ctx.CustomerPayments.FindAsync(model.PaymentId);
				if (entity == null)
				{
					TempData["Error"] = "पेमेंटची नोंद सापडली नाही.";
					return RedirectToAction(nameof(Index));
				}

				entity.PaymentDate = paymentDate;
				entity.CustomerId = model.CustomerId;
				entity.Amount = model.Amount;
				entity.PaymentModeId = model.PaymentModeId;
				entity.StatusId = model.StatusId;
				await _ctx.SaveChangesAsync();
				TempData["Success"] = "पेमेंट यशस्वीरित्या अपडेट झालं!";
			}

			return RedirectToAction(nameof(Index));
		}

		// ══ DELETE ───────────────────────────────────────────────────────────
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Delete(int id)
		{
			if (!IsLoggedIn()) return RedirectToAction("Index", "Login");

			var entity = await _ctx.CustomerPayments.FindAsync(id);
			if (entity != null)
			{
				_ctx.CustomerPayments.Remove(entity);
				await _ctx.SaveChangesAsync();
				TempData["Success"] = "पेमेंटची नोंद डिलीट झाली.";
			}
			else
			{
				TempData["Error"] = "पेमेंटची नोंद सापडली नाही.";
			}

			return RedirectToAction(nameof(Index));
		}
	}
}
