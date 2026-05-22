using System;
using System.Collections.Generic;

namespace Brick_Manufacturing_Management_System.DBContext;

public partial class LabourExpense
{
    public int ExpenseId { get; set; }

    public int? LabourId { get; set; }

    public DateOnly? ExpenseDate { get; set; }

    public decimal? Amount { get; set; }

    public string? Reason { get; set; }

    public virtual LabourMaster? Labour { get; set; }
}
