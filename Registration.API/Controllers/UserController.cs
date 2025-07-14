using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Registration.API.Business;
using Registration.API.Common;
using Registration.API.Entity.Dtos;
using Registration.API.Entity.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Registration.API.Controllers;

public class UserController(UserBusiness u, IMapper m, IOptions<AppSettings> ap, IHttpContextAccessor ac) :
    AdminGenericController<UserBusiness, User>(u, m, ap, ac)
{
    [HttpGet("{username}/{password}")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(string username, string password)
    {
        User? user = await Business.GetByUsername(username);
        if (user is null)
            return NotFound();

        var salt = CreateRandomString(32);
        var hashedPassword = HashPassword(password, user.PasswordSalt);

        if (hashedPassword != user.HashedPassword)
            return Forbid();

        return Ok(GenerateAdminToken(user, AppSetting));
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