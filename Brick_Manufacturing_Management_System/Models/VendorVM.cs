using Brick_Manufacturing_Management_System.DBContext;
using System.ComponentModel.DataAnnotations;

namespace Brick_Manufacturing_Management_System.Models
{
	public class VendorVM
	{
		public int VendorId { get; set; }

		[Required(ErrorMessage = "Vendor name is required.")]
		[StringLength(100)]
		[Display(Name = "Vendor Name")]
		public string VendorName { get; set; } = string.Empty;

		[StringLength(15)]
		[Required(ErrorMessage = "Mobile Number is required.")]
		[Display(Name = "Mobile Number")]
		[RegularExpression(@"^[0-9]{10,15}$", ErrorMessage = "Enter a valid mobile number.")]
		public string? MobileNumber { get; set; }

		[StringLength(200)]
		public string? Address { get; set; }

		[StringLength(20)]
		[Display(Name = "GST Number")]
		public string? GSTNumber { get; set; }

		// Nullable to match the DB column (bool? in VendorMaster entity)
		public bool? Status { get; set; } = true;

		// Full vendor list for the single-page layout
		public List<VendorMaster> VendorList { get; set; } = new();
	}
}
