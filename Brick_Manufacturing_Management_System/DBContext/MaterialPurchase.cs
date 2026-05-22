using System;
using System.Collections.Generic;

namespace Brick_Manufacturing_Management_System.DBContext;

public partial class MaterialPurchase
{
    public int PurchaseId { get; set; }

    public DateOnly PurchaseDate { get; set; }

    public int? VendorId { get; set; }

    public int? MaterialId { get; set; }

    public decimal? Quantity { get; set; }

    public decimal? Rate { get; set; }

    public decimal? TotalAmount { get; set; }

    public decimal? PaidAmount { get; set; }

    public decimal? PendingAmount { get; set; }

    public virtual MaterialMaster? Material { get; set; }

    public virtual VendorMaster? Vendor { get; set; }
}
