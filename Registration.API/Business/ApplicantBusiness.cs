using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Registration.API.Common;
using Registration.API.Entity.Dtos;

namespace Registration.API.Business;

public class ApplicantBusiness(RegContext context, IMapper mapper) : GenericBusiness<Applicant>(context, mapper)
{
    public async Task<ActionReport<TokenDto>> Signup(SignupDto signupForm, int regId, AppSettings appSetting)
    {
        Applicant? applicant = await FirstOrDefaultAsync(e =>
            e.RegId == regId && (e.NationalNumber == signupForm.NationalCode || e.PhoneNumber == signupForm.Mobile));
        TokenDto token;
        if (applicant is not null)
            if (!string.IsNullOrEmpty(applicant.TrackingCode))
                return ActionReport<TokenDto>.Error(HttpStatusCode.Conflict, "کد ملی یا شماره موبایل تکراری است. به صفحه پیگیری مراجعه بفرمایید");
            else
            {
                token = GenerateJwtTokenForApplicant(applicant.Id, signupForm.NationalCode, signupForm.Mobile, appSetting);
                return ActionReport<TokenDto>.Success(token);
            }


        applicant = new Applicant
        {
            RegId = regId,
            PhoneNumber = signupForm.Mobile,
            NationalNumber = signupForm.NationalCode,
        };
        ActionReport report = await Add(applicant);
        if (!report.Successful)
        {
            return ActionReport<TokenDto>.Error(report);
        }
        token = GenerateJwtTokenForApplicant(applicant.Id, signupForm.NationalCode, signupForm.Mobile, appSetting);
        return ActionReport<TokenDto>.Success(token);
    }
    public TokenDto GenerateJwtTokenForApplicant(int applicantId, string nationalCode, string mobile, AppSettings appSetting)
    {
        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSetting.SecretKey));
        SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        Claim[] claims =
        [
            new(ClaimTypes.Actor, "Applicant"),
            new(ClaimTypes.SerialNumber, applicantId.ToString()),
            new(ClaimTypes.NameIdentifier, nationalCode),
            new(ClaimTypes.MobilePhone, mobile)
        ];

        JwtSecurityToken token = new JwtSecurityToken(
            issuer: appSetting.Issuer,
            audience: appSetting.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(4),
            signingCredentials: credentials
        );

        TokenDto tokenDto = new()
        {
            TokenString = new JwtSecurityTokenHandler().WriteToken(token),
            RefreshToken = CreateRandomString(10),
        };
        return tokenDto;
    }
}