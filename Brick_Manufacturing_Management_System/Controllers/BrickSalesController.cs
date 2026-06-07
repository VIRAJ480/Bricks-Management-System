using Brick_Manufacturing_Management_System.DBContext;
using Brick_Manufacturing_Management_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Brick_Manufacturing_Management_System.Controllers
{
	public class BrickSalesController : Controller
	{
		private readonly BrickErpdbContext _ctx;

		public BrickSalesController(BrickErpdbContext context)
		{
			_ctx = context;
		}

		private bool IsLoggedIn() =>
			HttpContext.Session.GetString("Username") != null;

		private async Task PopulateDropdowns(BrickSalesVM vm)
		{
			vm.CustomerOptions = await _ctx.CustomerMasters
				.OrderBy(c => c.CustomerName)
				.Select(c => new SelectListItem
				{
					Value = c.CustomerId.ToString(),
					Text = c.CustomerName
				})
				.ToListAsync();

			vm.BrickTypeOptions = await _ctx.BrickTypes
				.Where(b => b.Status == true)
				.OrderBy(b => b.BrickTypeName)
				.Select(b => new SelectListItem
				{
					Value = b.BrickTypeId.ToString(),
					Text = b.BrickTypeName
				})
				.ToListAsync();
		}

		private async Task<List<BrickSalesListItem>> GetSalesList()
		{
			return await _ctx.BrickSales
				.Include(s => s.Customer)
				.OrderByDescending(s => s.SalesDate)
				.ThenByDescending(s => s.SalesId)
				.Select(s => new BrickSalesListItem
				{
					SalesId      = s.SalesId,
					SalesDate    = s.SalesDate ?? DateOnly.FromDateTime(DateTime.Today),
					CustomerName = s.Customer != null ? s.Customer.CustomerName ?? "—" : "—",
					BrickTypeName = _ctx.BrickTypes
						.Where(b => b.BrickTypeId.ToString() == s.BrickType)
						.Select(b => b.BrickTypeName)
						.FirstOrDefault() ?? s.BrickType ?? "—",
					Quantity     = s.Quantity ?? 0,
					Rate         = s.Rate ?? 0,
					TotalAmount  = s.TotalAmount ?? 0,
					PaidAmount   = s.PaidAmount ?? 0,
					PendingAmount = s.PendingAmount ?? 0
				})
				.ToListAsync();
		}

		[HttpGet]
		public async Task<IActionResult> Index(int? editId)
		{
			if (!IsLoggedIn()) return RedirectToAction("Index", "Login");

			var vm = new BrickSalesVM
			{
				SalesDate = DateOnly.FromDateTime(DateTime.Today)
			};

			await PopulateDropdowns(vm);
			vm.SalesList = await GetSalesList();

			if (editId.HasValue)
			{
				var entity = await _ctx.BrickSales.FindAsync(editId.Value);
				if (entity != null)
				{
					vm.SalesId       = entity.SalesId;
					vm.SalesDate     = entity.SalesDate;
					vm.CustomerId    = entity.CustomerId;
					vm.BrickTypeId   = int.TryParse(entity.BrickType, out int btId) ? btId : null;
					vm.Quantity      = entity.Quantity;
					vm.Rate          = entity.Rate;
					vm.TotalAmount   = entity.TotalAmount;
					vm.PaidAmount    = entity.PaidAmount;
					vm.PendingAmount = entity.PendingAmount;
				}
			}

			return View(vm);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Save(BrickSalesVM model)
		{
			if (!IsLoggedIn()) return RedirectToAction("Index", "Login");

			decimal qty     = model.Quantity ?? 0;
			decimal rate    = model.Rate ?? 0;
			decimal paid    = model.PaidAmount ?? 0;
			decimal total   = qty * rate;
			decimal pending = total - paid;

			model.TotalAmount   = total;
			model.PendingAmount = pending;

			await PopulateDropdowns(model);
			model.SalesList = await GetSalesList();

			if (paid > total)
			{
				ModelState.AddModelError("PaidAmount", "दिलेली रक्कम एकूण रकमेपेक्षा जास्त असू शकत नाही.");
				return View("Index", model);
			}

			if (!ModelState.IsValid)
				return View("Index", model);

			DateOnly salesDate = model.SalesDate!.Value;

			if (model.SalesId == 0)
			{
				var entity = new BrickSale
				{
					SalesDate     = salesDate,
					CustomerId    = model.CustomerId,
					BrickType     = model.BrickTypeId.ToString(),
					Quantity      = (int)qty,
					Rate          = rate,
					TotalAmount   = total,
					PaidAmount    = paid,
					PendingAmount = pending
				};
				_ctx.BrickSales.Add(entity);
				await _ctx.SaveChangesAsync();
				TempData["Success"] = $"विक्री यशस्वीरित्या नोंद झाली. एकूण रक्कम: ₹{total:N2}";
			}
			else
			{
				var entity = await _ctx.BrickSales.FindAsync(model.SalesId);
				if (entity == null)
				{
					TempData["Error"] = "Sale record not found.";
					return RedirectToAction(nameof(Index));
				}

				entity.SalesDate     = salesDate;
				entity.CustomerId    = model.CustomerId;
				entity.BrickType     = model.BrickTypeId.ToString();
				entity.Quantity      = (int)qty;
				entity.Rate          = rate;
				entity.TotalAmount   = total;
				entity.PaidAmount    = paid;
				entity.PendingAmount = pending;
				await _ctx.SaveChangesAsync();
				TempData["Success"] = $"विक्री यशस्वीरित्या अपडेट झाली. एकूण रक्कम: ₹{total:N2}";
			}

			return RedirectToAction(nameof(Index));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Delete(int id)
		{
			if (!IsLoggedIn()) return RedirectToAction("Index", "Login");

			var entity = await _ctx.BrickSales.FindAsync(id);
			if (entity != null)
			{
				_ctx.BrickSales.Remove(entity);
				await _ctx.SaveChangesAsync();
				TempData["Success"] = "विक्रीची नोंद डिलीट झाली.";
			}
			else
			{
				TempData["Error"] = "विक्रीची नोंद सापडली नाही.";
			}

			return RedirectToAction(nameof(Index));
		}
	}
}
