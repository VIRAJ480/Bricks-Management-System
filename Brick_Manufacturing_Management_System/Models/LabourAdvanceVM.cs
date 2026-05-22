using Brick_Manufacturing_Management_System.DBContext;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Brick_Manufacturing_Management_System.Models
{
	public class LabourAdvanceVM
	{
		public int AdvanceId { get; set; }

		[Required(ErrorMessage = "Please select a labour.")]
		[Display(Name = "Labour")]
		public int? LabourId { get; set; }

		[Required(ErrorMessage = "Advance date is required.")]
		[Display(Name = "Advance Date")]
		[DataType(DataType.Date)]
		public DateOnly? AdvanceDate { get; set; }

		[Required(ErrorMessage = "Please select an advance type.")]
		[Display(Name = "Advance Type")]
		public int? AdvanceTypeId { get; set; }

		[Required(ErrorMessage = "Advance amount is required.")]
		[Range(1, 9999999.99, ErrorMessage = "Amount must be greater than 0.")]
		[Display(Name = "Advance Amount")]
		public decimal? AdvanceAmount { get; set; }

		// Dropdowns
		public List<SelectListItem> LabourOptions { get; set; } = new();
		public List<SelectListItem> AdvanceTypeOptions { get; set; } = new();

		// Full list for table
		public List<LabourAdvanceListItem> AdvanceList { get; set; } = new();
	}

	// Flat DTO for the table rows (avoids lazy-load issues)
	public class LabourAdvanceListItem
	{
		public int AdvanceId { get; set; }
		public string LabourName { get; set; } = string.Empty;
		public DateOnly AdvanceDate { get; set; }
		public string AdvanceTypeName { get; set; } = string.Empty;
		public decimal AdvanceAmount { get; set; }
		public int LabourId { get; set; }
		// Running total advance for this labour (computed in controller)
		public decimal TotalAdvance { get; set; }
	}
}
