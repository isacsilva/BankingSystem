using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Core;

public partial class Bankaccount
{
    public int Id { get; set; }

    public string Branch { get; set; } = null!;

    public string Number { get; set; }

    public string Type { get; set; } = null!;

    public string HolderName { get; set; } = null!;

    public string? HolderEmail { get; set; }

    public string HolderDocument { get; set; } = null!;

    public string HolderType { get; set; } = null!;

    public string Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [JsonIgnore]
    public virtual Balance? Balance { get; set; }

    [JsonIgnore]
    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
