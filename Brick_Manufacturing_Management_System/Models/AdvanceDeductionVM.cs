using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Brick_Manufacturing_Management_System.Models
{
	public class AdvanceDeductionVM
	{
		public int DeductionId { get; set; }

		[Required(ErrorMessage = "Deduction date is required.")]
		[Display(Name = "Deduction Date")]
		[DataType(DataType.Date)]
		public DateOnly? DeductionDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

		[Required(ErrorMessage = "Please select a labour.")]
		[Display(Name = "Labour")]
		public int? LabourId { get; set; }

		[Required(ErrorMessage = "Deduction amount is required.")]
		[Range(0.01, 999999999.99, ErrorMessage = "Amount must be greater than 0.")]
		[Display(Name = "Deduction Amount")]
		public decimal? Amount { get; set; }

		[Required(ErrorMessage = "Reason is required.")]
		[StringLength(200, ErrorMessage = "Reason cannot exceed 200 characters.")]
		public string? Reason { get; set; }

		// Read-only display — total advance taken by this labour
		public decimal CurrentAdvance { get; set; } = 0;

		// Read-only display — advance balance after deduction
		public decimal AdvanceBalance { get; set; } = 0;

		// Dropdown options
		public List<SelectListItem> LabourOptions { get; set; } = new();

		// Table list
		public List<AdvanceDeductionListItem> DeductionList { get; set; } = new();
	}

	public class AdvanceDeductionListItem
	{
		public int DeductionId { get; set; }
		public DateOnly DeductionDate { get; set; }
		public string LabourName { get; set; } = string.Empty;
		public decimal Amount { get; set; }
		public string Reason { get; set; } = string.Empty;
	}
}
