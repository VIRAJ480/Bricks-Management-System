using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Brick_Manufacturing_Management_System.Models
{
	public class BrickSalesVM
	{
		public int SalesId { get; set; }

		[Required(ErrorMessage = "Sales date is required.")]
		[Display(Name = "Sales Date")]
		[DataType(DataType.Date)]
		public DateOnly? SalesDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

		[Required(ErrorMessage = "Please select a customer.")]
		[Display(Name = "Customer")]
		public int? CustomerId { get; set; }

		[Required(ErrorMessage = "Please select a brick type.")]
		[Display(Name = "Brick Type")]
		public int? BrickTypeId { get; set; }

		[Required(ErrorMessage = "Quantity is required.")]
		[Range(1, 9999999, ErrorMessage = "Quantity must be greater than 0.")]
		public int? Quantity { get; set; }

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

		// Dropdowns
		public List<SelectListItem> CustomerOptions { get; set; } = new();
		public List<SelectListItem> BrickTypeOptions { get; set; } = new();

		// List for table
		public List<BrickSalesListItem> SalesList { get; set; } = new();
	}

	public class BrickSalesListItem
	{
		public int SalesId { get; set; }
		public DateOnly SalesDate { get; set; }
		public string CustomerName { get; set; } = string.Empty;
		public string BrickTypeName { get; set; } = string.Empty;
		public int Quantity { get; set; }
		public decimal Rate { get; set; }
		public decimal TotalAmount { get; set; }
		public decimal PaidAmount { get; set; }
		public decimal PendingAmount { get; set; }
	}
}
