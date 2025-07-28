using System;
using System.Collections.Generic;

namespace Registration.API.Entity.Models;

public partial class RegCost
{
    public override int Id { get; set; }

    public override DateTime CreatedDate { get; set; }

    public int RegId { get; set; }

    public string Title { get; set; } = null!;

    public int Amount { get; set; }

    public string? Description { get; set; }

    public virtual Reg Reg { get; set; } = null!;
}
