using System;
using System.Collections.Generic;

namespace Brick_Manufacturing_Management_System.DBContext;

public partial class PaymentMode
{
    public int PaymentModeId { get; set; }

    public string PaymentModeName { get; set; } = null!;

    public bool? Status { get; set; }

    public virtual ICollection<CustomerPayment> CustomerPayments { get; set; } = new List<CustomerPayment>();

    public virtual ICollection<VendorPayment> VendorPayments { get; set; } = new List<VendorPayment>();
}
