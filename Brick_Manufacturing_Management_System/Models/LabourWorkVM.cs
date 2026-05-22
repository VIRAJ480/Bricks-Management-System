using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Brick_Manufacturing_Management_System.Models
{
	public class LabourWorkVM
	{
		public int WorkId { get; set; }

		[Required(ErrorMessage = "Work date is required.")]
		[Display(Name = "Work Date")]
		[DataType(DataType.Date)]
		public DateOnly? WorkDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

		[Required(ErrorMessage = "Please select a labour.")]
		[Display(Name = "Labour")]
		public int? LabourId { get; set; }

		[Required(ErrorMessage = "Daily wage is required.")]
		[Range(0.01, 999999.99, ErrorMessage = "Daily wage must be greater than 0.")]
		[Display(Name = "Daily Wage")]
		public decimal? DailyWage { get; set; }

		[Required(ErrorMessage = "Days worked is required.")]
		[Range(1, 365, ErrorMessage = "Days worked must be between 1 and 365.")]
		[Display(Name = "Days Worked")]
		public int? DaysWorked { get; set; }

		// Auto-calculated: DailyWage × DaysWorked
		public decimal? TotalSalary { get; set; }

		// Dropdown options
		public List<SelectListItem> LabourOptions { get; set; } = new();

		// Table list
		public List<LabourWorkListItem> WorkList { get; set; } = new();
	}

	public class LabourWorkListItem
	{
		public int WorkId { get; set; }
		public DateOnly WorkDate { get; set; }
		public string LabourName { get; set; } = string.Empty;
		public decimal DailyWage { get; set; }
		public int DaysWorked { get; set; }
		public decimal TotalSalary { get; set; }
	}
}
