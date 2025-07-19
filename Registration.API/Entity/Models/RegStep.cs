using System;
using System.Collections.Generic;

namespace Registration.API.Entity.Models;

public partial class RegStep
{
    public override int Id { get; set; }

    public int RegId { get; set; }

    public int StepId { get; set; }

    public bool? CreateTrackingCode { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public byte Order { get; set; }

    public byte? MemberLimit { get; set; }

    public string? AddMemberDescription { get; set; }

    public override DateTime CreatedDate { get; set; }

    public virtual ICollection<ApplicantFormValue> ApplicantFormValues { get; set; } = new List<ApplicantFormValue>();

    public virtual ICollection<Field> Fields { get; set; } = new List<Field>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Reg Reg { get; set; } = null!;

    public virtual ICollection<RegStepStatus> RegStepStatuses { get; set; } = new List<RegStepStatus>();

    public virtual Step Step { get; set; } = null!;
}
