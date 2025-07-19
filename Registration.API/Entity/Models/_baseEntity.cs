namespace Registration.API.Entity.Models;

public abstract class BaseEntity
{
    public abstract int Id { get; set; }
    public abstract DateTime CreatedDate { get; set; }
}

partial class ApplicantFormValue : BaseEntity;
partial class Applicant : BaseEntity;
partial class FieldOption : BaseEntity;
partial class Field : BaseEntity;
partial class FieldType : BaseEntity;
partial class Order : BaseEntity;
partial class Payment : BaseEntity;
partial class Reg : BaseEntity;
partial class RegStep : BaseEntity;
partial class RegStepStatus : BaseEntity;
partial class Step : BaseEntity;
//
partial class User : BaseEntity;
