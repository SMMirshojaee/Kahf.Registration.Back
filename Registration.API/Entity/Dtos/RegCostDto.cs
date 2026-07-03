namespace Registration.API.Entity.Dtos;

public class RegCostDto
{
    public int Id { get; set; }

    public int RegId { get; set; }

    public string Title { get; set; } = null!;

    public int Amount { get; set; }

    public string? Description { get; set; }
}

public class ApplicantExtraCostDto
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public int Amount { get; set; }

    public string? Description { get; set; }
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

    public string? TrackingCode { get; set; }

    public int MembersCount { get; set; }

    public List<OrderDto> Orders { get; set; } = new();

    public List<ApplicantExtraCostDto> ApplicantExtraCosts { get; set; } = new();

}