namespace Registration.API.Entity.Dtos;

public class ApplicantFormValueDto
{
    public int Id { get; set; }

    public int ApplicantId { get; set; }

    public int RegStepId { get; set; }

    public int FieldId { get; set; }

    public string? Value { get; set; }

    public int? FieldOptionId { get; set; }

    public bool Deleted { get; set; }

}

public class ApplicantWithFormValueDto
{
    public int Id { get; set; }

    public DateTime CreatedDate { get; set; }

    public int RegId { get; set; }

    public int? StatusId { get; set; }

    public string NationalNumber { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? Description { get; set; }

    public int? LeaderId { get; set; }

    public string? TrackingCode { get; set; }

    public virtual List<ApplicantInfoDto> InverseLeader { get; set; } = new List<ApplicantInfoDto>();

    public virtual List<ApplicantFormValueDto> ApplicantFormValues { get; set; } = new();

    public RegStepStatusDto? Status { get; set; }
}