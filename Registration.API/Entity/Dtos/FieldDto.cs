namespace Registration.API.Entity.Dtos;

public class FieldDto
{
    public int Id { get; set; }

    public int RegStepId { get; set; }

    public int FieldTypeId { get; set; }

    public FieldTypeDto FieldType { get; set; } = null!;

    public short Order { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public bool Mandatory { get; set; }

    public bool ForLeader { get; set; }

    public bool ForMember { get; set; }

    public bool Hidden { get; set; }

    public virtual ICollection<FieldOptionDto> FieldOptions { get; set; } = new List<FieldOptionDto>();
}

public class FieldOptionDto
{
    public int Id { get; set; }

    public int FieldId { get; set; }

    public string Title { get; set; } = null!;

    public string Type { get; set; } = null!;

    public string? Value { get; set; }
}

public class FieldTypeDto
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Name { get; set; } = null!;

}