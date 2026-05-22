using System;
using System.Collections.Generic;

namespace Brick_Manufacturing_Management_System.DBContext;

public partial class CustomerMaster
{
    public int CustomerId { get; set; }

    public string? CustomerName { get; set; }

    public string? MobileNumber { get; set; }

    public string? Address { get; set; }

    public virtual ICollection<BrickSale> BrickSales { get; set; } = new List<BrickSale>();

    public virtual ICollection<CustomerPayment> CustomerPayments { get; set; } = new List<CustomerPayment>();
}
