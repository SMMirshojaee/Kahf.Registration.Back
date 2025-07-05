using System;
using System.Collections.Generic;

namespace Registration.API.Entity.Models;

public partial class ApplicantFormValues
{
    public override int Id { get; set; }

    public override DateTime CreatedDate { get; set; }

    public int ApplicantId { get; set; }

    public int FieldId { get; set; }

    public string? Value { get; set; }

    public int? FieldOptionId { get; set; }

    public bool Deleted { get; set; }

    public virtual Applicants Applicant { get; set; } = null!;

    public virtual Fields Field { get; set; } = null!;

    public virtual FieldOptions? FieldOption { get; set; }
}
