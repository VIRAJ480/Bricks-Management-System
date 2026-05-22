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

		// Auto-fetched from LabourWork: SUM(DailyWage * DaysWorked) for selected labour
		public decimal? TotalSalary { get; set; }

		// Auto-fetched from LabourExpense: SUM(Amount) for selected labour
		public decimal? TotalExpense { get; set; }

		// Auto-calculated: TotalSalary - TotalExpense
		public decimal? FinalSalary { get; set; }

		[Required(ErrorMessage = "Paid amount is required.")]
		[Range(0, 999999999.99, ErrorMessage = "Paid amount cannot be negative.")]
		[Display(Name = "Paid Amount")]
		public decimal? PaidAmount { get; set; } = 0;

		[Required(ErrorMessage = "Please select a payment status.")]
		[Display(Name = "Payment Status")]
		public int? StatusId { get; set; }

		// ── Dropdowns ─────────────────────────────────────────────────────────
		public List<SelectListItem> LabourOptions { get; set; } = new();
		public List<SelectListItem> StatusOptions { get; set; } = new();

		// ── Table list ────────────────────────────────────────────────────────
		public List<SalaryPaymentListItem> SalaryList { get; set; } = new();
	}

	public class SalaryPaymentListItem
	{
		public int SalaryId { get; set; }
		public string LabourName { get; set; } = string.Empty;
		public DateOnly SalaryDate { get; set; }
		public decimal TotalSalary { get; set; }
		public decimal TotalExpense { get; set; }
		public decimal FinalSalary { get; set; }
		public decimal PaidAmount { get; set; }
		public string StatusName { get; set; } = string.Empty;
	}
}
