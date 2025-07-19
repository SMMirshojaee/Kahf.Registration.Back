using System;
using System.Collections.Generic;

namespace Registration.API.Entity.Models;

public partial class Payment
{
    public override int Id { get; set; }

    public override DateTime CreatedDate { get; set; }

    public int RegStepId { get; set; }

    public int PerPersonAmount { get; set; }

    public string? Description { get; set; }

    public virtual RegStep RegStep { get; set; } = null!;
}
