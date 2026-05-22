using System;
using System.Collections.Generic;

namespace Brick_Manufacturing_Management_System.DBContext;

public partial class AdvanceType
{
    public int AdvanceTypeId { get; set; }

    public string? AdvanceTypeName { get; set; }

    public virtual ICollection<LabourAdvance> LabourAdvances { get; set; } = new List<LabourAdvance>();
}
