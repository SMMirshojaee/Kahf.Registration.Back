namespace Registration.API.Entity.Dtos
{
    public class SignupDto
    {
        
        public required string NationalCode { get; set; } = null!;
        public required string Mobile { get; set; } = null!;
    }
}
