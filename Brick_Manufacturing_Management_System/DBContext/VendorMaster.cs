using System;
using System.Collections.Generic;

namespace Brick_Manufacturing_Management_System.DBContext;

public partial class VendorMaster
{
    public int VendorId { get; set; }

    public string VendorName { get; set; } = null!;

    public string? MobileNumber { get; set; }

    public string? Address { get; set; }

    public string? Gstnumber { get; set; }

    public bool? Status { get; set; }

    public virtual ICollection<MaterialPurchase> MaterialPurchases { get; set; } = new List<MaterialPurchase>();

    public virtual ICollection<VendorPayment> VendorPayments { get; set; } = new List<VendorPayment>();
}
