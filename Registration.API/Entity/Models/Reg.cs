using System;
using System.Collections.Generic;

namespace Registration.API.Entity.Models;

public partial class Reg
{
    public override int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public bool IsActive { get; set; }

    public override DateTime CreatedDate { get; set; }

    public virtual ICollection<RegStep> RegSteps { get; set; } = new List<RegStep>();
}
