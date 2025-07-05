namespace Registration.API.Entity.Models;

public abstract class BaseEntity
{
    public abstract int Id { get; set; }
    public abstract DateTime CreatedDate { get; set; }
}

partial class Applicants : BaseEntity;
partial class Fields : BaseEntity;
partial class FieldTypes : BaseEntity;
partial class Regs : BaseEntity;
partial class RegSteps : BaseEntity;
partial class RegStepStatuses : BaseEntity;
partial class Steps : BaseEntity;