using System;
using System.Collections.Generic;

namespace Registration.API.Entity.Models;

public partial class RegStepStatus
{
    public override int Id { get; set; }

    public int RegStepId { get; set; }

    public string Title { get; set; } = null!;

    public string? PublicMessage { get; set; }

    public bool IsWaiting { get; set; }

    public bool IsNotChecked { get; set; }

    public bool IsAccepted { get; set; }

    public bool IsReserved { get; set; }

    public bool IsRejected { get; set; }

    public override DateTime CreatedDate { get; set; }

    public virtual ICollection<Applicant> Applicants { get; set; } = new List<Applicant>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual RegStep RegStep { get; set; } = null!;
}
