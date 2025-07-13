using System;
using System.Collections.Generic;

namespace Registration.API.Entity.Models;

public partial class FieldOption
{
    public override int Id { get; set; }

    public int FieldId { get; set; }

    public string Title { get; set; } = null!;

    public string Type { get; set; } = null!;

    public string? Value { get; set; }

    public override DateTime CreatedDate { get; set; }

    public virtual ICollection<ApplicantFormValue> ApplicantFormValues { get; set; } = new List<ApplicantFormValue>();

    public virtual Field Field { get; set; } = null!;
}
