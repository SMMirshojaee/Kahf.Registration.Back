namespace Registration.API.Entity.Models;

public partial class ApplicantExtraCost
{
    public override int Id { get; set; }

    public override DateTime CreatedDate { get; set; }

    public int ApplicantId { get; set; }

    public string Title { get; set; } = null!;

    public int Amount { get; set; }

    public string? Description { get; set; }

    public virtual Applicant Applicant { get; set; } = null!;
}
