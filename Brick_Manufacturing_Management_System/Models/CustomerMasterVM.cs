using System.ComponentModel.DataAnnotations;

namespace Brick_Manufacturing_Management_System.Models
{
	public class CustomerMasterVM
	{
		public int CustomerId { get; set; }

		[Required(ErrorMessage = "Customer name is required.")]
		[StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
		[Display(Name = "Customer Name")]
		public string? CustomerName { get; set; }

		[Required(ErrorMessage = "Mobile number is required.")]
		[StringLength(15, ErrorMessage = "Mobile number cannot exceed 15 characters.")]
		[RegularExpression(@"^[0-9]{10,15}$", ErrorMessage = "Enter a valid mobile number (10–15 digits).")]
		[Display(Name = "Mobile Number")]
		public string? MobileNumber { get; set; }

		[StringLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
		public string? Address { get; set; }

		// Table list
		public List<CustomerMasterListItem> CustomerList { get; set; } = new();
	}

	public class CustomerMasterListItem
	{
		public int CustomerId { get; set; }
		public string CustomerName { get; set; } = string.Empty;
		public string? MobileNumber { get; set; }
		public string? Address { get; set; }
	}
}
