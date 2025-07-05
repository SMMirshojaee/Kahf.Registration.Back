using System;
using System.Collections.Generic;

namespace Registration.API.Entity.Models;

public partial class RegSteps
{
    public override int Id { get; set; }

    public int RegId { get; set; }

    public int StepId { get; set; }

    public string Title { get; set; } = null!;

    public byte Order { get; set; }

    public override DateTime CreatedDate { get; set; }

    public virtual ICollection<Fields> Fields { get; set; } = new List<Fields>();

    public virtual Regs Reg { get; set; } = null!;

    public virtual ICollection<RegStepStatuses> RegStepStatuses { get; set; } = new List<RegStepStatuses>();

    public virtual Steps Step { get; set; } = null!;
}
