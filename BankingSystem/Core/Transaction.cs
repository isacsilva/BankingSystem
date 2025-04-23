using System;
using System.Collections.Generic;

namespace Core;

public partial class Transaction
{
    public int Id { get; set; }

    public string Type { get; set; } = null!;

    public decimal Amount { get; set; }

    public int BankAccountId { get; set; }

    public string? CounterpartyBankCode { get; set; }

    public string? CounterpartyBankName { get; set; }

    public string? CounterpartyBranch { get; set; }

    public string? CounterpartyAccountNumber { get; set; }

    public string? CounterpartyAccountType { get; set; }

    public string? CounterpartyHolderName { get; set; }

    public string? CounterpartyHolderType { get; set; }

    public string? CounterpartyHolderDocument { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Bankaccount BankAccount { get; set; } = null!;
}
