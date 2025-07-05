using System;
using System.Collections.Generic;

namespace Registration.API.Entity.Models;

public partial class Fields
{
    public short Id { get; set; }

    public short RegStepId { get; set; }

    public short FieldTypeId { get; set; }

    public short Order { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public bool Mandatory { get; set; }

    public bool Hidden { get; set; }

    public DateTime CreatedDate { get; set; }

    public virtual FieldTypes FieldType { get; set; } = null!;

    public virtual RegSteps RegStep { get; set; } = null!;
}
