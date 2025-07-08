namespace Registration.API.Entity.Dtos
{
    public class RegDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public string? ImageAddress { get; set; } = null!;

        public string? Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsActive { get; set; }

        public virtual ICollection<RegStepDto> RegSteps { get; set; } = new List<RegStepDto>();
    }

    public class RegStepDto
    {
        public int Id { get; set; }

        public int RegId { get; set; }

        public int StepId { get; set; }

        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public byte Order { get; set; }

        public virtual ICollection<RegStepStatusDto> RegStepStatuses { get; set; } = new List<RegStepStatusDto>();
    }

    public class RegStepStatusDto
    {
        public int Id { get; set; }

        public int RegStepId { get; set; }

        public string Title { get; set; } = null!;

        public bool IsNotChecked { get; set; }

        public bool IsAccepted { get; set; }

        public bool IsReserved { get; set; }

        public bool IsRejected { get; set; }

    }

    public class StepDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public string Description { get; set; } = null!;

        public virtual ICollection<RegStepDto> RegSteps { get; set; } = new List<RegStepDto>();
    }
}
