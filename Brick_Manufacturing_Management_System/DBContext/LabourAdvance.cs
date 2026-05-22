using System;
using System.Collections.Generic;

namespace Brick_Manufacturing_Management_System.DBContext;

public partial class LabourAdvance
{
    public int AdvanceId { get; set; }

    public int? LabourId { get; set; }

    public DateOnly? AdvanceDate { get; set; }

    public decimal? AdvanceAmount { get; set; }

    public int? AdvanceTypeId { get; set; }

    public virtual AdvanceType? AdvanceType { get; set; }

    public virtual LabourMaster? Labour { get; set; }
}
