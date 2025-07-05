using System;
using System.Collections.Generic;

namespace Registration.API.Entity.Models;

public partial class Fields
{
    public override int Id { get; set; }

    public int RegStepId { get; set; }

    public int FieldTypeId { get; set; }

    public short Order { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public bool Mandatory { get; set; }

    public bool Hidden { get; set; }

    public override DateTime CreatedDate { get; set; }

    public virtual ICollection<ApplicantFormValues> ApplicantFormValues { get; set; } = new List<ApplicantFormValues>();

    public virtual FieldTypes FieldType { get; set; } = null!;

    public virtual RegSteps RegStep { get; set; } = null!;
}
