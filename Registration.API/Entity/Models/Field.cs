namespace Registration.API.Entity.Models;

public partial class Field
{
    public override int Id { get; set; }

    public int RegStepId { get; set; }

    public int FieldTypeId { get; set; }

    public short Order { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public bool Mandatory { get; set; }

    public bool Hidden { get; set; }

    public bool ForLeader { get; set; }

    public bool ForMember { get; set; }

    public override DateTime CreatedDate { get; set; }

    public virtual ICollection<ApplicantFormValue> ApplicantFormValues { get; set; } = new List<ApplicantFormValue>();

    public virtual ICollection<FieldOption> FieldOptions { get; set; } = new List<FieldOption>();

    public virtual FieldType FieldType { get; set; } = null!;

    public virtual RegStep RegStep { get; set; } = null!;
}
