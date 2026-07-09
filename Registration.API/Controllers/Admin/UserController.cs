using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Registration.API.Business;
using Registration.API.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Registration.API.Controllers.Admin;

public class UserController(
	SmsHelper smsHelper,
	UserBusiness u,
	IMapper m,
	IOptions<AppSettings> ap,
	IHttpContextAccessor ac,
	ILoggerFactory lg) :
	AdminGenericController<UserBusiness, User>(u, m, ap, ac, lg)
{
	private SmsHelper _smsHelper => smsHelper;

	[HttpGet("{username}/{password}")]
	[AllowAnonymous]
	public async Task<IActionResult> Login(string username, string password)
	{
		User? user = await Business.GetByUsername(username);
		if (user is null)
			return NotFound();

		//var salt = CreateRandomString(32);
		var hashedPassword = HashPassword(password, user.PasswordSalt);

		if (hashedPassword != user.HashedPassword)
			return Forbid();

		return Ok(GenerateAdminToken(user, AppSetting));
	}

	[HttpGet]
	public async Task<IActionResult> SendSms()
	{
		//SmsIr sms = new();
		//await sms.SendBuck("سلام عزیزم", ["09128486146", "09198114632"]);

		//var message = await _smsSender.Send("تست پیامک سامانه کهف", "09128486146", "989919228231");
		//return Ok(message);
		return NotFound();
	}


	private string GenerateAdminToken(User user, AppSettings appSetting)
	{
		SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSetting.SecretKey));
		SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

		Claim[] claims =
		[
			new(ClaimTypes.Actor, user.Role),
			new(ClaimTypes.Name, user.Username),
			new(ClaimTypes.NameIdentifier, user.Id.ToString()),
		];

		JwtSecurityToken token = new JwtSecurityToken(
			issuer: appSetting.Issuer,
			audience: appSetting.Audience,
			claims: claims,
			expires: DateTime.UtcNow.AddHours(24),
			signingCredentials: credentials
		);

		return new JwtSecurityTokenHandler().WriteToken(token);
	}
}