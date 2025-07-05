using System;
using System.Collections.Generic;

namespace Registration.API.Entity.Models;

public partial class FieldTypes
{
    public override int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Name { get; set; } = null!;

    public override DateTime CreatedDate { get; set; }

    public virtual ICollection<Fields> Fields { get; set; } = new List<Fields>();
}
