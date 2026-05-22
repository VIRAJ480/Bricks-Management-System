using System;
using System.Collections.Generic;

namespace Brick_Manufacturing_Management_System.DBContext;

public partial class MaterialMaster
{
    public int MaterialId { get; set; }

    public string MaterialName { get; set; } = null!;

    public bool? Status { get; set; }

    public int? UnitId { get; set; }

    public virtual ICollection<MaterialPurchase> MaterialPurchases { get; set; } = new List<MaterialPurchase>();

    public virtual Unit? Unit { get; set; }
}
