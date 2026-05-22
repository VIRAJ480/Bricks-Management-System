using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Brick_Manufacturing_Management_System.Models
{
	public class MaterialPurchaseVM
	{
		public int PurchaseId { get; set; }

		[Required(ErrorMessage = "Purchase date is required.")]
		[Display(Name = "Purchase Date")]
		[DataType(DataType.Date)]
		public DateOnly? PurchaseDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

		[Required(ErrorMessage = "Please select a vendor.")]
		[Display(Name = "Vendor")]
		public int? VendorId { get; set; }

		[Required(ErrorMessage = "Please select a material.")]
		[Display(Name = "Material")]
		public int? MaterialId { get; set; }

		[Required(ErrorMessage = "Quantity is required.")]
		[Range(0.01, 999999.99, ErrorMessage = "Quantity must be greater than 0.")]
		public decimal? Quantity { get; set; }

		[Required(ErrorMessage = "Rate is required.")]
		[Range(0.01, 999999.99, ErrorMessage = "Rate must be greater than 0.")]
		public decimal? Rate { get; set; }

		// Auto-calculated: Quantity × Rate
		public decimal? TotalAmount { get; set; }

		[Range(0, 999999999.99, ErrorMessage = "Paid amount cannot be negative.")]
		[Display(Name = "Paid Amount")]
		public decimal? PaidAmount { get; set; } = 0;

		// Auto-calculated: TotalAmount - PaidAmount
		public decimal? PendingAmount { get; set; }

		// ── For dropdowns & list ──────────────────────────────────────────────
		public List<SelectListItem> VendorOptions { get; set; } = new();
		public List<SelectListItem> MaterialOptions { get; set; } = new();

		// Full purchase list for the right-side table
		public List<MaterialPurchaseListItem> PurchaseList { get; set; } = new();
	}

	// ── Flat DTO for displaying the table (avoids heavy EF includes in view) ──
	public class MaterialPurchaseListItem
	{
		public int PurchaseId { get; set; }
		public DateOnly PurchaseDate { get; set; }
		public string VendorName { get; set; } = string.Empty;
		public string MaterialName { get; set; } = string.Empty;
		public decimal Quantity { get; set; }
		public decimal Rate { get; set; }
		public decimal TotalAmount { get; set; }
		public decimal PaidAmount { get; set; }
		public decimal PendingAmount { get; set; }
	}
}
