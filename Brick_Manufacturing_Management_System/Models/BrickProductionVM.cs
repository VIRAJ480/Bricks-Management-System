using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Brick_Manufacturing_Management_System.Models
{
	public class BrickProductionVM
	{
		public int ProductionId { get; set; }

		[Required(ErrorMessage = "Production date is required.")]
		[Display(Name = "Production Date")]
		[DataType(DataType.Date)]
		public DateOnly? ProductionDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

		[Required(ErrorMessage = "Please select a brick type.")]
		[Display(Name = "Brick Type")]
		public int? BrickTypeId { get; set; }

		[Required(ErrorMessage = "Quantity is required.")]
		[Range(1, 9999999, ErrorMessage = "Quantity must be at least 1.")]
		public int? Quantity { get; set; }

		// ── Dropdowns & list ─────────────────────────────────────────────────
		public List<SelectListItem> BrickTypeOptions { get; set; } = new();
		public List<BrickProductionListItem> ProductionList { get; set; } = new();
	}

	public class BrickProductionListItem
	{
		public int ProductionId { get; set; }
		public DateOnly ProductionDate { get; set; }
		public string BrickTypeName { get; set; } = string.Empty;
		public int Quantity { get; set; }
	}
}
