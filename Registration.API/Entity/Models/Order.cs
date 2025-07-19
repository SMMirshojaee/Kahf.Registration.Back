using System;
using System.Collections.Generic;

namespace Registration.API.Entity.Models;

public partial class Order
{
    public override int Id { get; set; }

    public override DateTime CreatedDate { get; set; }

    public int ApplicantId { get; set; }

    public int RegStepId { get; set; }

    public string NationalNumber { get; set; } = null!;

    public int Amount { get; set; }

    public string? RequestContent { get; set; }

    public int? RequestStatus { get; set; }

    public DateTime? RequestDate { get; set; }

    public string? Authority { get; set; }

    public string? VerifyContent { get; set; }

    public int? VerifyStatus { get; set; }

    public DateTime? VerifyDate { get; set; }

    public long? RefId { get; set; }
}
