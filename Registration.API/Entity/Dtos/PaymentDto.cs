namespace Registration.API.Entity.Dtos;

public class PaymentDto
{
    public int RegStepId { get; set; }

    public int PerPersonAmount { get; set; }

    public string? Description { get; set; }

    public byte? InstallmentsCount { get; set; }

}

public class OrderDto
{
    public DateTime CreatedDate { get; set; }

    public int ApplicantId { get; set; }

    public int RegStepId { get; set; }

    public int Amount { get; set; }

    public int? RequestStatus { get; set; }

    public DateTime? RequestDate { get; set; }

    public string? Authority { get; set; }

    public int? VerifyStatus { get; set; }

    public DateTime? VerifyDate { get; set; }

    public long? RefId { get; set; }
}

public class ApplicantOrderDto
{
    public int Id { get; set; }
    public int? StatusId { get; set; }
    public string? StatusTitle { get; set; }
    public string? StepTitle { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string NationalNumber { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public int MembersCount { get; set; }
    public List<OrderDto> Orders { get; set; } = new();
}