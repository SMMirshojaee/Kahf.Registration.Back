namespace Registration.API.Entity.Dtos;

public class ApplicantFormValueDto
{
    public int Id { get; set; }

    public int ApplicantId { get; set; }

    public int FieldId { get; set; }

    public string? Value { get; set; }

    public int? FieldOptionId { get; set; }

    public bool Deleted { get; set; }

    public virtual FieldDto Field { get; set; } = null!;

    public virtual FieldOptionDto? FieldOption { get; set; }
}