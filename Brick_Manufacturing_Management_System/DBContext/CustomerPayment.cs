using System;
using System.Collections.Generic;

namespace Brick_Manufacturing_Management_System.DBContext;

public partial class CustomerPayment
{
    public int PaymentId { get; set; }

    public int? CustomerId { get; set; }

    public DateOnly? PaymentDate { get; set; }

    public decimal? Amount { get; set; }

    public int? PaymentModeId { get; set; }

    public int? StatusId { get; set; }

    public virtual CustomerMaster? Customer { get; set; }

    public virtual PaymentMode? PaymentMode { get; set; }

    public virtual PaymentStatus? Status { get; set; }
}
