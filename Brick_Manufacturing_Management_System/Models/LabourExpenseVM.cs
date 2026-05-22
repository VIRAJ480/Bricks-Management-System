using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Brick_Manufacturing_Management_System.Models
{
	public class LabourExpenseVM
	{
		public int ExpenseId { get; set; }

		[Required(ErrorMessage = "Expense date is required.")]
		[Display(Name = "Expense Date")]
		[DataType(DataType.Date)]
		public DateOnly? ExpenseDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

		[Required(ErrorMessage = "Please select a labour.")]
		[Display(Name = "Labour")]
		public int? LabourId { get; set; }

		[Required(ErrorMessage = "Amount is required.")]
		[Range(0.01, 999999999.99, ErrorMessage = "Amount must be greater than 0.")]
		public decimal? Amount { get; set; }

		[Required(ErrorMessage = "Reason is required.")]
		[StringLength(200, ErrorMessage = "Reason cannot exceed 200 characters.")]
		public string? Reason { get; set; }

		// Dropdown options
		public List<SelectListItem> LabourOptions { get; set; } = new();

		// Table list
		public List<LabourExpenseListItem> ExpenseList { get; set; } = new();
	}

	public class LabourExpenseListItem
	{
		public int ExpenseId { get; set; }
		public DateOnly ExpenseDate { get; set; }
		public string LabourName { get; set; } = string.Empty;
		public decimal Amount { get; set; }
		public string Reason { get; set; } = string.Empty;
	}
}
