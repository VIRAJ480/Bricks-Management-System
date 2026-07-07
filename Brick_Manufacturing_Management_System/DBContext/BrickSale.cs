using System;
using System.Collections.Generic;

namespace Brick_Manufacturing_Management_System.DBContext;

public partial class BrickSale
{
    public int SalesId { get; set; }

    public DateOnly? SalesDate { get; set; }

    public int? CustomerId { get; set; }

    public int? BrickTypeId { get; set; }

    public int? Quantity { get; set; }

    public decimal? Rate { get; set; }

    public decimal? TotalAmount { get; set; }

    public decimal? PaidAmount { get; set; }

    public decimal? PendingAmount { get; set; }

    public virtual CustomerMaster? Customer { get; set; }
}
