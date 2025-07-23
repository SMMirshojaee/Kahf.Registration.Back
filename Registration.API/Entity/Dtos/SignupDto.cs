namespace Registration.API.Entity.Dtos;

public class SignupDto
{
    public required string FirstName{ get; set; } = null!;
    public required string LastName { get; set; } = null!;
    public required string NationalCode { get; set; } = null!;
    public required string Mobile { get; set; } = null!;
}
public class SigninDto
{
    public required string NationalCode { get; set; } = null!;
    public required string Mobile { get; set; } = null!;
    public required string TrackingCode { get; set; } = null!;
}