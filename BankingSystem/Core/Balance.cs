using System;
using System.Collections.Generic;

namespace Core;

public partial class Balance
{
    public int BankAccountId { get; set; }

    public decimal AvailableAmount { get; set; }

    public decimal BlockedAmount { get; set; }
}
