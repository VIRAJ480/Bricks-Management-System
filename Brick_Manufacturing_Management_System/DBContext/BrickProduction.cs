using System;
using System.Collections.Generic;

namespace Brick_Manufacturing_Management_System.DBContext;

public partial class BrickProduction
{
    public int ProductionId { get; set; }

    public DateOnly? ProductionDate { get; set; }

    public int? Quantity { get; set; }

    public int? BrickTypeId { get; set; }

    public virtual BrickType? BrickType { get; set; }
}
