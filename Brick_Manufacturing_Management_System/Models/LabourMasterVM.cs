using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Brick_Manufacturing_Management_System.DBContext;

namespace Brick_Manufacturing_Management_System.Models
{
	public class LabourMasterVM
	{
		public int LabourId { get; set; }

		[Required(ErrorMessage = "Labour name is required.")]
		[StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
		public string LabourName { get; set; } = string.Empty;

		[Required(ErrorMessage = "Mobile number is required.")]
		[StringLength(15, ErrorMessage = "Mobile number cannot exceed 15 digits.")]
		[RegularExpression(@"^\d{10,15}$", ErrorMessage = "Enter a valid mobile number.")]
		public string? MobileNumber { get; set; }

		[StringLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
		public string? Address { get; set; }

		[Range(0, 999999.99, ErrorMessage = "Daily wage must be between 0 and 9,99,999.")]
		public decimal? DailyWage { get; set; }

		public DateOnly? JoiningDate { get; set; }   // ✅ matches entity

		public List<LabourMaster> LabourList { get; set; } = new();
	}
}