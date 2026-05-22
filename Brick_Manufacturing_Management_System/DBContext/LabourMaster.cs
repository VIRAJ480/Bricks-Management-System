using System;
using System.Collections.Generic;

namespace Brick_Manufacturing_Management_System.DBContext;

public partial class LabourMaster
{
    public int LabourId { get; set; }

    public string LabourName { get; set; } = null!;

    public string? MobileNumber { get; set; }

    public string? Address { get; set; }

    public decimal? DailyWage { get; set; }

    public DateOnly? JoiningDate { get; set; }

    public virtual ICollection<AdvanceDeduction> AdvanceDeductions { get; set; } = new List<AdvanceDeduction>();

    public virtual ICollection<LabourAdvance> LabourAdvances { get; set; } = new List<LabourAdvance>();

    public virtual ICollection<LabourExpense> LabourExpenses { get; set; } = new List<LabourExpense>();

    public virtual ICollection<LabourWork> LabourWorks { get; set; } = new List<LabourWork>();

    public virtual ICollection<SalaryPayment> SalaryPayments { get; set; } = new List<SalaryPayment>();
}
