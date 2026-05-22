using System;
using System.Collections.Generic;

namespace Brick_Manufacturing_Management_System.DBContext;

public partial class BrickType
{
    public int BrickTypeId { get; set; }

    public string BrickTypeName { get; set; } = null!;

    public bool? Status { get; set; }

    public virtual ICollection<BrickProduction> BrickProductions { get; set; } = new List<BrickProduction>();
}
