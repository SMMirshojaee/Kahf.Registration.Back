using System;
using System.Collections.Generic;

namespace Registration.API.Entity.Models;

public partial class RegStepStatuses
{
    public override short Id { get; set; }

    public short RegStepId { get; set; }

    public string Title { get; set; } = null!;

    public bool IsNotChecked { get; set; }

    public bool IsAccepted { get; set; }

    public bool IsReserved { get; set; }

    public bool IsRejected { get; set; }

    public override DateTime CreatedDate { get; set; }

    public virtual RegSteps RegStep { get; set; } = null!;
}
