using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Core;

public partial class Balance
{
    public int BankAccountId { get; set; }

    public decimal AvailableAmount { get; set; }

    public decimal BlockedAmount { get; set; }

    [JsonIgnore]
    public virtual Bankaccount BankAccount { get; set; } = null!;
}
