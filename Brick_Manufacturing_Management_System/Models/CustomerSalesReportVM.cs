namespace Brick_Manufacturing_Management_System.Models
{
	public class CustomerSalesReportVM
	{
		public DateTime? FromDate { get; set; }
		public DateTime? ToDate { get; set; }

		public List<CustomerSalesReportRow> ReportData { get; set; } = new();

		public decimal GrossTotal { get; set; }
	}

	public class CustomerSalesReportRow
	{
		public DateTime SalesDate { get; set; }
		public string CustomerName { get; set; } = string.Empty;
		public string BrickTypeName { get; set; } = string.Empty;
		public decimal Quantity { get; set; }
		public decimal Rate { get; set; }
		public decimal TotalAmount { get; set; }
		public decimal Gross { get; set; }
	}
}
