namespace Registration.API.Entity.Dtos
{
    public class ApplicantDto
    {
        public DateTime CreatedDate { get; set; }
        public int? StatusId { get; set; }
        public int? RegStepId { get; set; }
        public string? Title { get; set; }
        public bool? IsNotChecked { get; set; }
        public bool? IsAccepted { get; set; }
        public bool? IsReserved { get; set; }
        public bool? IsRejected { get; set; }
    }
}
