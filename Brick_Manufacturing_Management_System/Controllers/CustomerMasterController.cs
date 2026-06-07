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
				TempData["Error"] = $"कस्टमरचे रेकॉर्ड लोड करताना प्रॉब्लेम आला: {ex.Message}";
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
						TempData["Error"] = "कस्टमरची नोंद सापडली नाही.";
					}
				}
				catch (Exception ex)
				{
					TempData["Error"] = $"एडिटसाठी रेकॉर्ड लोड करताना प्रॉब्लेम आला: {ex.Message}";
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
					TempData["Success"] = $"'{model.CustomerName}' कस्टमर यशस्वीरित्या अॅड झाला.";
				}
				else
				{
					// ── UPDATE ──
					var entity = await _ctx.CustomerMasters.FindAsync(model.CustomerId);
					if (entity == null)
					{
						TempData["Error"] = "कस्टमरची नोंद सापडली नाही. कदाचित ती डिलीट झाली असेल.";
						return RedirectToAction(nameof(Index));
					}

					entity.CustomerName = model.CustomerName?.Trim();
					entity.MobileNumber = model.MobileNumber?.Trim();
					entity.Address = model.Address?.Trim();
					await _ctx.SaveChangesAsync();
					TempData["Success"] = $"'{model.CustomerName}' कस्टमर यशस्वीरित्या अपडेट झाला.";
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
				TempData["Error"] = "रेकॉर्डचा ID चुकीचा आहे.";
				return RedirectToAction(nameof(Index));
			}

			try
			{
				var entity = await _ctx.CustomerMasters.FindAsync(id);
				if (entity == null)
				{
					TempData["Error"] = "कस्टमरची नोंद सापडली नाही. कदाचित ती आधीच डिलीट झाली असेल.";
					return RedirectToAction(nameof(Index));
				}

				bool hasSales    = await _ctx.BrickSales.AnyAsync(x => x.CustomerId == id);
				bool hasPayments = await _ctx.CustomerPayments.AnyAsync(x => x.CustomerId == id);

				if (hasSales || hasPayments)
				{
					var linked = new List<string>();
					if (hasSales)    linked.Add("Brick Sales");
					if (hasPayments) linked.Add("Customer Payment");
					TempData["Error"] = $"'{entity.CustomerName}' ला डिलीट करता येणार नाही — {string.Join(", ", linked)} मध्ये याचे रेकॉर्ड आधीपासून आहेत. आधी ते रेकॉर्ड डिलीट करा.";
					return RedirectToAction(nameof(Index));
				}

				_ctx.CustomerMasters.Remove(entity);
				await _ctx.SaveChangesAsync();
				TempData["Success"] = $"'{entity.CustomerName}' कस्टमर यशस्वीरित्या डिलीट झाला.";
			}
			catch (DbUpdateException dbEx)
			{
				TempData["Error"] = $"डिलीट करता आलं नाही: {dbEx.InnerException?.Message ?? dbEx.Message}";
			}
			catch (Exception ex)
			{
				TempData["Error"] = $"डिलीट करताना अचानक प्रॉब्लेम आला: {ex.Message}";
			}

			return RedirectToAction(nameof(Index));
		}
	}
}
