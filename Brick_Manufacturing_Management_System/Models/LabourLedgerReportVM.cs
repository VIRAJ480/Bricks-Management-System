using Microsoft.AspNetCore.Mvc.Rendering;

namespace Brick_Manufacturing_Management_System.Models
{
	public class LabourLedgerReportVM
	{
		// ── Filter inputs ──────────────────────────────────────────────────
		public int? LabourId { get; set; }
		public DateTime? FromDate { get; set; } = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
		public DateTime? ToDate { get; set; } = DateTime.Today;

		// ── Dropdown ───────────────────────────────────────────────────────
		public List<SelectListItem> LabourOptions { get; set; } = new();

		// ── Report result (null = SP returned no rows) ─────────────────────
		public LabourLedgerResult? Result { get; set; }

		// ── Was the filter form submitted? ─────────────────────────────────
		public bool ReportRequested { get; set; } = false;
	}

	/// <summary>
	/// Keyless DTO — column names must match sp_GetLabourLedger output exactly.
	/// Registered in DbContext with HasNoKey().ToView(null).
	/// </summary>
	public class LabourLedgerResult
	{
		public int LabourId { get; set; }
		public string LabourName { get; set; } = string.Empty;

		// Advance section
		public decimal InitialAdvance { get; set; }
		public decimal AdditionalAdvance { get; set; }
		public decimal TotalAdvance { get; set; }
		public decimal TotalDeduction { get; set; }
		public decimal AdvanceBalance { get; set; }

		// Salary section
		public decimal SalaryEarned { get; set; }
		public decimal TotalExpense { get; set; }
		public decimal FinalSalary { get; set; }
		public decimal PaidAmount { get; set; }
	}
}