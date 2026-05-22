namespace Brick_Manufacturing_Management_System.Models
{
	// ── Maps directly to the SP result set (keyless entity) ───────────────
	public class SalesReportSpResult
	{
		public int TotalCustomers { get; set; }
		public decimal TotalRevenue { get; set; }
		public DateTime FromDate { get; set; }
		public DateTime ToDate { get; set; }
		public DateTime ReportGeneratedOn { get; set; }
	}

	// ── ViewModel bound to the Razor view ─────────────────────────────────
	public class SalesReportVM
	{
		// Filter inputs
		public DateTime FromDate { get; set; } = new DateTime(DateTime.Today.Year, 1, 1);
		public DateTime ToDate { get; set; } = DateTime.Today;

		// Report state
		public bool IsGenerated { get; set; } = false;

		// KPI values from SP
		public int TotalCustomers { get; set; }
		public decimal TotalRevenue { get; set; }
		public DateTime? ReportGeneratedOn { get; set; }
	}
}
