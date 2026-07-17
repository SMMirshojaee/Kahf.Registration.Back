namespace Registration.API.Entity.Dtos;

public class PaymentDto
{
    public int RegStepId { get; set; }

    public int PerPersonAmount { get; set; }

    public string? Description { get; set; }

    public byte? InstallmentsCount { get; set; }

}

public class OrderDto
{
    public int Id { get; set; }

    public DateTime CreatedDate { get; set; }

    public int ApplicantId { get; set; }

    public int RegStepId { get; set; }

    public int Amount { get; set; }

    public int? RequestStatus { get; set; }

    public DateTime? RequestDate { get; set; }

    public string? Authority { get; set; }

    public int? VerifyStatus { get; set; }

    public DateTime? VerifyDate { get; set; }

    public long? RefId { get; set; }

    public long? LoanId { get; set; }

    public string? Description { get; set; }

    public ICollection<OrderDto>? InverseLoan { get; set; }

	public required string NationalNumber { get; set; }
    }

public class InstallmentDto
{
    public required int Amount { get; set; }
    public required DateTime Date { get; set; }
    public required int ApplicantId { get; set; }
    public required int LoanId { get; set; }
    public required string NationalNumber { get; set; }
    public string? Description { get; set; }
}
