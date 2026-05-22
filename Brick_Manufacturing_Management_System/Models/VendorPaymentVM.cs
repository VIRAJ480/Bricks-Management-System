using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Brick_Manufacturing_Management_System.Models
{
	public class VendorPaymentVM
	{
		public int PaymentId { get; set; }

		[Required(ErrorMessage = "Payment date is required.")]
		[Display(Name = "Payment Date")]
		[DataType(DataType.Date)]
		public DateOnly? PaymentDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

		[Required(ErrorMessage = "Please select a vendor.")]
		[Display(Name = "Vendor")]
		public int? VendorId { get; set; }

		[Required(ErrorMessage = "Amount is required.")]
		[Range(0.01, 999999999.99, ErrorMessage = "Amount must be greater than 0.")]
		public decimal? Amount { get; set; }

		[Required(ErrorMessage = "Please select a payment mode.")]
		[Display(Name = "Payment Mode")]
		public int? PaymentModeId { get; set; }

		[Required(ErrorMessage = "Please select a status.")]
		[Display(Name = "Payment Status")]
		public int? StatusId { get; set; }

		// ── Dropdowns ──────────────────────────────────────────────────────
		public List<SelectListItem> VendorOptions { get; set; } = new();
		public List<SelectListItem> PaymentModeOptions { get; set; } = new();
		public List<SelectListItem> StatusOptions { get; set; } = new();

		// ── Right-side table list ───────────────────────────────────────────
		public List<VendorPaymentListItem> PaymentList { get; set; } = new();
	}

	// ── Flat DTO for the right-side table ──────────────────────────────────
	public class VendorPaymentListItem
	{
		public int PaymentId { get; set; }
		public DateOnly PaymentDate { get; set; }
		public string VendorName { get; set; } = string.Empty;
		public decimal Amount { get; set; }
		public string PaymentMode { get; set; } = string.Empty;
		public string StatusName { get; set; } = string.Empty;
	}
}
