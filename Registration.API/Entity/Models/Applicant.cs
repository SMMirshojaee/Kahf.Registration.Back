using System;
using System.Collections.Generic;

namespace Registration.API.Entity.Models;

public partial class Applicant
{
    public override int Id { get; set; }

    public override DateTime CreatedDate { get; set; }

    public short RegId { get; set; }

    public short StatusId { get; set; }

    public string NationalNumber { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string TrackingCode { get; set; } = null!;

    public virtual ICollection<ApplicantFormValue> ApplicantFormValues { get; set; } = new List<ApplicantFormValue>();
}
