using System.ComponentModel.DataAnnotations;

namespace Brick_Manufacturing_Management_System.Models
{
	public class LoginVM
	{
		[Required(ErrorMessage = "Username is required.")]
		[StringLength(50)]
		public string Username { get; set; } = string.Empty;

		[Required(ErrorMessage = "Password is required.")]
		[StringLength(100)]
		public string Password { get; set; } = string.Empty;

		public bool RememberMe { get; set; }
	}
}
