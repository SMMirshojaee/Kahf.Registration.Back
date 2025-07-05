namespace Registration.API.Entity.Models;

public abstract class BaseEntity
{
    public abstract short Id { get; set; }
    public abstract DateTime CreatedDate { get; set; }
}

partial class Regs : BaseEntity;
partial class RegSteps : BaseEntity;
partial class Steps : BaseEntity;
partial class RegStepStatuses : BaseEntity;