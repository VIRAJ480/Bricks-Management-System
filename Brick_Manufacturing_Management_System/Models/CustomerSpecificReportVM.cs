using Microsoft.AspNetCore.Mvc.Rendering;

namespace Brick_Manufacturing_Management_System.Models
{
	public class CustomerSpecificReportVM
	{
		public int? CustomerId { get; set; }
		public string? CustomerName { get; set; }

		public DateTime? FromDate { get; set; }
		public DateTime? ToDate { get; set; }

		public List<SelectListItem> CustomerOptions { get; set; } = new();

		public List<CustomerSpecificReportRow> ReportData { get; set; } = new();

		public decimal GrossTotal { get; set; }
	}

	public class CustomerSpecificReportRow
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
