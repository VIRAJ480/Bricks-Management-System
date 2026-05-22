namespace Brick_Manufacturing_Management_System.Models
{
	public class VendorPendingPaymentVM
	{
		public int VendorId { get; set; }
		public string VendorName { get; set; } = string.Empty;
		public decimal FinalAmount { get; set; }
		public decimal TotalPaid { get; set; }
		public decimal RemainingAmount { get; set; }
		public string PaymentStatus { get; set; } = string.Empty;

		// Computed helper — safe against divide-by-zero
		public int PaidPercent => FinalAmount > 0
			? (int)Math.Min(100, Math.Round((TotalPaid / FinalAmount) * 100))
			: 0;
	}
}