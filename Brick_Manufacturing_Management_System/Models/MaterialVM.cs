using Brick_Manufacturing_Management_System.DBContext;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Brick_Manufacturing_Management_System.Models
{
	public class MaterialVM
	{
		public int MaterialId { get; set; }

		[Required(ErrorMessage = "Material name is required.")]
		[StringLength(100)]
		[Display(Name = "Material Name")]
		public string MaterialName { get; set; } = string.Empty;

		[Required(ErrorMessage = "Please select a unit.")]
		[Display(Name = "Unit")]
		public int? UnitId { get; set; }

		// Nullable to match BIT (bool?) in DB — same as VendorMaster.Status
		public bool? Status { get; set; } = true;

		// ── Populated by controller for the view ──────────────────────────────
		public List<MaterialMaster> MaterialList { get; set; } = new();
		public List<SelectListItem> UnitOptions { get; set; } = new();
	}
}
