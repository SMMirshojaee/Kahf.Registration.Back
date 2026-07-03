using System;
using System.Collections.Generic;

namespace Registration.API.Entity.Models;

public partial class Message
{
    public override int Id { get; set; }

    public override DateTime CreatedDate { get; set; }

    public string Text { get; set; } = null!;

    public int? ApplicantId { get; set; }

    public string Mobile { get; set; } = null!;

    public string NationalNumber { get; set; } = null!;

    public int? Status { get; set; }

    public int? UserId { get; set; }

    public virtual Applicant? Applicant { get; set; }

    public virtual User? User { get; set; }
}
