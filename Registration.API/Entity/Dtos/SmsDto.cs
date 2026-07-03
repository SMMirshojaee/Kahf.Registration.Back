namespace Registration.API.Entity.Dtos;

public class SmsDto
{
    public required string Text { get; set; }

    public required List<int> ApplicantIds { get; set; }
}