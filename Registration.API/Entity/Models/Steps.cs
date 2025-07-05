using System;
using System.Collections.Generic;

namespace Registration.API.Entity.Models;

public partial class Steps
{
    public override short Id { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public override DateTime CreatedDate { get; set; }

    public virtual ICollection<RegSteps> RegSteps { get; set; } = new List<RegSteps>();
}
