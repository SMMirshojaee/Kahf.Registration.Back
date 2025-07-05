using System;
using System.Collections.Generic;

namespace Registration.API.Entity.Models;

public partial class FieldTypes
{
    public short Id { get; set; }

    public string Title { get; set; } = null!;

    public string Name { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public virtual ICollection<Fields> Fields { get; set; } = new List<Fields>();
}
