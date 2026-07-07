using Brick_Manufacturing_Management_System.DBContext;
using System.ComponentModel.DataAnnotations;

namespace Brick_Manufacturing_Management_System.Models
{
	public class BricksTypeVM
	{
		public int BrickTypeId { get; set; }

		[Required(ErrorMessage = "Brick Type name is required.")]
		[StringLength(100)]
		[Display(Name = "Brick Type Name")]
		public string BrickTypeName { get; set; } = string.Empty;

		public bool? Status { get; set; } = true;

		public List<BrickType> BrickTypeList { get; set; } = new();
	}
}