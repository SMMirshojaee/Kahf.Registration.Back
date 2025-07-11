namespace Registration.API.Entity.Dtos
{
    public class ApplicantDto
    {
        public DateTime CreatedDate { get; set; }
        public int? StatusId { get; set; }
        public int? RegStepId { get; set; }
        public string? Title { get; set; }
        public bool? IsWaiting { get; set; }
        public bool? IsNotChecked { get; set; }
        public bool? IsAccepted { get; set; }
        public bool? IsReserved { get; set; }
        public bool? IsRejected { get; set; }
    }
    public class MemberInfoDto
    {
        public int Id { get; set; }
        public int StatusId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string NationalNumber { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
    }
}
