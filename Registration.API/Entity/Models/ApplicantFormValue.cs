using System;
using System.Collections.Generic;

namespace Registration.API.Entity.Models;

public partial class ApplicantFormValue
{
    public override int Id { get; set; }

    public override DateTime CreatedDate { get; set; }

    public int ApplicantId { get; set; }

    public int RegStepId { get; set; }

    public int FieldId { get; set; }

    public string? Value { get; set; }

    public int? FieldOptionId { get; set; }

    public bool Deleted { get; set; }

    public virtual Applicant Applicant { get; set; } = null!;

    public virtual Field Field { get; set; } = null!;

    public virtual FieldOption? FieldOption { get; set; }

    public virtual RegStep RegStep { get; set; } = null!;
}
