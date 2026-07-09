namespace Registration.API.Entity.Models;

public partial class Order
{
    public override int Id { get; set; }

    public override DateTime CreatedDate { get; set; }

    public int? ApplicantId { get; set; }

    public int? RegStepId { get; set; }

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

    public int? LoanId { get; set; }

    public string? Description { get; set; }

    public int? UserId { get; set; }

    public virtual Applicant? Applicant { get; set; }

    public virtual ICollection<Order> InverseLoan { get; set; } = new List<Order>();

    public virtual Order? Loan { get; set; }

    public virtual RegStep? RegStep { get; set; }

    public virtual User? User { get; set; }
}
