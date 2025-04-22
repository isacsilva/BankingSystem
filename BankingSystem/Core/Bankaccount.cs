using System;
using System.Collections.Generic;

namespace Core;

public partial class Bankaccount
{
    public int Id { get; set; }

    public string Branch { get; set; } = null!;

    public string Number { get; set; } = null!;

    public string Type { get; set; } = null!;

    public string HolderName { get; set; } = null!;

    public string? HolderEmail { get; set; }

    public string HolderDocument { get; set; } = null!;

    public string HolderType { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
