namespace Registration.API.Entity.Dtos;

public class TokenDto
{
    public string TokenString { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
}