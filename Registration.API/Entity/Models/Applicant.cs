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

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public int? LeaderId { get; set; }

    public string? TrackingCode { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<ApplicantFormValue> ApplicantFormValues { get; set; } = new List<ApplicantFormValue>();

    public virtual ICollection<Applicant> InverseLeader { get; set; } = new List<Applicant>();

    public virtual Applicant? Leader { get; set; }

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual Reg Reg { get; set; } = null!;

    public virtual RegStepStatus? Status { get; set; }
}
