using System;
using System.Collections.Generic;

namespace Brick_Manufacturing_Management_System.DBContext;

public partial class LabourWork
{
    public int WorkId { get; set; }

    public int? LabourId { get; set; }

    public DateOnly? WorkDate { get; set; }

    public int? DaysWorked { get; set; }

    public decimal? DailyWage { get; set; }

    public decimal? TotalSalary { get; set; }

    public virtual LabourMaster? Labour { get; set; }
}
