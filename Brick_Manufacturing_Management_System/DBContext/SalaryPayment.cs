using System.ComponentModel.DataAnnotations.Schema;
namespace Brick_Manufacturing_Management_System.DBContext;

public partial class SalaryPayment
{
	public int SalaryId { get; set; }
	public int? LabourId { get; set; }
	public DateOnly? SalaryDate { get; set; }
	public decimal? DailyWage { get; set; }

	// DaysWorked column in DB is INT — must match
	public int? WorkingDays { get; set; }

	public decimal? TotalSalary { get; set; }
	public decimal? TotalExpense { get; set; }
	public decimal? FinalSalary { get; set; }
	public decimal? PaidAmount { get; set; }
	public int? StatusId { get; set; }

	public virtual LabourMaster? Labour { get; set; }
	public virtual PaymentStatus? Status { get; set; }
}