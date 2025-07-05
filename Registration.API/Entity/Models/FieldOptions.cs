using System;
using System.Collections.Generic;

namespace Registration.API.Entity.Models;

public partial class FieldOptions
{
    public override int Id { get; set; }

    public int FieldId { get; set; }

    public string Title { get; set; } = null!;

    public string Type { get; set; } = null!;

    public string Value { get; set; } = null!;

    public override DateTime CreatedDate { get; set; }

    public virtual ICollection<ApplicantFormValues> ApplicantFormValues { get; set; } = new List<ApplicantFormValues>();

    public virtual Fields Field { get; set; } = null!;
}
