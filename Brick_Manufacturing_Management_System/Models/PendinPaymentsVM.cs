namespace Brick_Manufacturing_Management_System.Models
{
	public class PendingPaymentsVM
	{
		public int CustomerId { get; set; }
		public string CustomerName { get; set; } = string.Empty;
		public decimal FinalAmount { get; set; }
		public decimal TotalPaid { get; set; }
		public decimal RemainingAmount { get; set; }
		public string PaymentStatus { get; set; } = string.Empty;

		// Computed helper
		public int PaidPercent => FinalAmount > 0
			? (int)Math.Min(100, Math.Round((TotalPaid / FinalAmount) * 100))
			: 0;
	}
}
