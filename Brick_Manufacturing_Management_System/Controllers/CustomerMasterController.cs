using Brick_Manufacturing_Management_System.DBContext;
using Brick_Manufacturing_Management_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Brick_Manufacturing_Management_System.Controllers
{
	public class CustomerMasterController : Controller
	{
		private readonly BrickErpdbContext _ctx;

		public CustomerMasterController(BrickErpdbContext context)
		{
			_ctx = context;
		}

		// ── Auth guard ────────────────────────────────────────────────────────
		private bool IsLoggedIn() =>
			HttpContext.Session.GetString("Username") != null;

		// ── Build flat customer list for table ────────────────────────────────
		private async Task<List<CustomerMasterListItem>> GetCustomerList()
		{
			try
			{
				return await _ctx.CustomerMasters
					.OrderBy(c => c.CustomerName)
					.Select(c => new CustomerMasterListItem
					{
						CustomerId = c.CustomerId,
						CustomerName = c.CustomerName ?? string.Empty,
						MobileNumber = c.MobileNumber,
						Address = c.Address
					})
					.ToListAsync();
			}
			catch (Exception ex)
			{
				TempData["Error"] = $"Failed to load customer records: {ex.Message}";
				return new List<CustomerMasterListItem>();
			}
		}

		// ══ INDEX ─────────────────────────────────────────────────────────────
		[HttpGet]
		public async Task<IActionResult> Index(int? editId)
		{
			if (!IsLoggedIn()) return RedirectToAction("Index", "Login");

			var vm = new CustomerMasterVM();
			vm.CustomerList = await GetCustomerList();

			if (editId.HasValue)
			{
				try
				{
					var entity = await _ctx.CustomerMasters
						.FirstOrDefaultAsync(c => c.CustomerId == editId.Value);

					if (entity != null)
					{
						vm.CustomerId = entity.CustomerId;
						vm.CustomerName = entity.CustomerName;
						vm.MobileNumber = entity.MobileNumber;
						vm.Address = entity.Address;
					}
					else
					{
						TempData["Error"] = "Customer record not found.";
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
		public async Task<IActionResult> Save(CustomerMasterVM model)
		{
			if (!IsLoggedIn()) return RedirectToAction("Index", "Login");

			// Rebuild list for re-render on error
			model.CustomerList = await GetCustomerList();

			if (!ModelState.IsValid)
				return View("Index", model);

			try
			{
				if (model.CustomerId == 0)
				{
					// ── INSERT ──
					var entity = new CustomerMaster
					{
						CustomerName = model.CustomerName?.Trim(),
						MobileNumber = model.MobileNumber?.Trim(),
						Address = model.Address?.Trim()
					};
					_ctx.CustomerMasters.Add(entity);
					await _ctx.SaveChangesAsync();
					TempData["Success"] = $"Customer '{model.CustomerName}' added successfully.";
				}
				else
				{
					// ── UPDATE ──
					var entity = await _ctx.CustomerMasters.FindAsync(model.CustomerId);
					if (entity == null)
					{
						TempData["Error"] = "Customer record not found. It may have been deleted.";
						return RedirectToAction(nameof(Index));
					}

					entity.CustomerName = model.CustomerName?.Trim();
					entity.MobileNumber = model.MobileNumber?.Trim();
					entity.Address = model.Address?.Trim();
					await _ctx.SaveChangesAsync();
					TempData["Success"] = $"Customer '{model.CustomerName}' updated successfully.";
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
				var entity = await _ctx.CustomerMasters.FindAsync(id);
				if (entity == null)
				{
					TempData["Error"] = "Customer record not found. It may have already been deleted.";
					return RedirectToAction(nameof(Index));
				}

				bool hasSales    = await _ctx.BrickSales.AnyAsync(x => x.CustomerId == id);
				bool hasPayments = await _ctx.CustomerPayments.AnyAsync(x => x.CustomerId == id);

				if (hasSales || hasPayments)
				{
					var linked = new List<string>();
					if (hasSales)    linked.Add("Brick Sales");
					if (hasPayments) linked.Add("Customer Payment");
					TempData["Error"] = $"Cannot delete '{entity.CustomerName}' — linked records exist in: {string.Join(", ", linked)}. Please delete those records first.";
					return RedirectToAction(nameof(Index));
				}

				_ctx.CustomerMasters.Remove(entity);
				await _ctx.SaveChangesAsync();
				TempData["Success"] = $"Customer '{entity.CustomerName}' deleted successfully.";
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
