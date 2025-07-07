using System;
using System.Collections.Generic;

namespace Registration.API.Entity.Models;

public partial class Applicant
{
    public override int Id { get; set; }

    public override DateTime CreatedDate { get; set; }

    public int RegId { get; set; }

    public int? StatusId { get; set; }

    public string NationalNumber { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string? TrackingCode { get; set; }

    public virtual ICollection<ApplicantFormValue> ApplicantFormValues { get; set; } = new List<ApplicantFormValue>();

    public virtual Reg Reg { get; set; } = null!;

    public virtual RegStepStatus? Status { get; set; }
}
