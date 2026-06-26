using Microsoft.AspNetCore.Mvc.Rendering;

namespace Brick_Manufacturing_Management_System.Models
{
	public class VendorSpecificMaterialPurchaseVM
	{
		public int? VendorId { get; set; }
		public string? VendorName { get; set; }

		public DateTime? FromDate { get; set; }
		public DateTime? ToDate { get; set; }

		public List<SelectListItem> VendorOptions { get; set; } = new();

		public List<VendorSpecificMaterialPurchaseRow> ReportData { get; set; } = new();

		public decimal GrossTotal { get; set; }
	}

	public class VendorSpecificMaterialPurchaseRow
	{
		public DateTime PurchaseDate { get; set; }
		public string VendorName { get; set; } = string.Empty;
		public string MaterialName { get; set; } = string.Empty;
		public decimal Quantity { get; set; }
		public decimal Rate { get; set; }
		public decimal TotalAmount { get; set; }
		public decimal Gross { get; set; }
	}
}
