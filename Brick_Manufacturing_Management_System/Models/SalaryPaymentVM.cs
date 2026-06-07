using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Brick_Manufacturing_Management_System.Models
{
	public class SalaryPaymentVM
	{
		public int SalaryId { get; set; }

		[Required(ErrorMessage = "Please select a labour.")]
		[Display(Name = "Labour")]
		public int? LabourId { get; set; }

		[Required(ErrorMessage = "Salary date is required.")]
		[Display(Name = "Salary Date")]
		[DataType(DataType.Date)]
		public DateOnly? SalaryDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

		[Required(ErrorMessage = "Daily wage is required.")]
		[Range(0, 999999999.99, ErrorMessage = "Daily wage cannot be negative.")]
		[Display(Name = "Daily Wage (₹)")]
		public decimal? DailyWage { get; set; } = 0;

		[Required(ErrorMessage = "Working days is required.")]
		[Range(0, 366, ErrorMessage = "Working days must be between 0 and 366.")]
		[Display(Name = "Working Days")]
		public int? WorkingDays { get; set; } = 0;

		// Auto-calculated: DailyWage * WorkingDays
		public decimal? TotalSalary { get; set; }

		[Required(ErrorMessage = "Total expense is required.")]
		[Range(0, 999999999.99, ErrorMessage = "Total expense cannot be negative.")]
		[Display(Name = "Total Expense (₹)")]
		public decimal? TotalExpense { get; set; } = 0;

		// Auto-calculated: TotalSalary - TotalExpense
		public decimal? FinalSalary { get; set; }

		[Required(ErrorMessage = "Paid amount is required.")]
		[Range(0, 999999999.99, ErrorMessage = "Paid amount cannot be negative.")]
		[Display(Name = "Paid Amount")]
		public decimal? PaidAmount { get; set; } = 0;

		[Required(ErrorMessage = "Please select a payment status.")]
		[Display(Name = "Payment Status")]
		public int? StatusId { get; set; }

		public List<SelectListItem> LabourOptions { get; set; } = new();
		public List<SelectListItem> StatusOptions { get; set; } = new();
		public List<SalaryPaymentListItem> SalaryList { get; set; } = new();
	}

	public class SalaryPaymentListItem
	{
		public int SalaryId { get; set; }
		public string LabourName { get; set; } = string.Empty;
		public DateOnly SalaryDate { get; set; }
		public decimal DailyWage { get; set; }
		public int WorkingDays { get; set; }
		public decimal TotalSalary { get; set; }
		public decimal TotalExpense { get; set; }
		public decimal FinalSalary { get; set; }
		public decimal PaidAmount { get; set; }
		public string StatusName { get; set; } = string.Empty;
	}
}