using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Registration.API.Common;
using Registration.API.Entity.Dtos;

namespace Registration.API.Business;

public class ApplicantBusiness(RegStepBusiness regStepBusiness, RegContext context, IMapper mapper) : GenericBusiness<Applicant>(context, mapper)
{
    public async Task<ActionReport<TokenDto>> Signup(int regId, string firstName, string lastName, string nationalCode, string mobile, AppSettings appSetting)
    {
        Applicant? applicant = await FirstOrDefaultAsync(e =>
            e.RegId == regId && (e.NationalNumber == nationalCode || e.PhoneNumber == mobile));
        TokenDto token;
        if (applicant is not null)
            if (!string.IsNullOrEmpty(applicant.TrackingCode))
                return ActionReport<TokenDto>.Error(HttpStatusCode.Conflict, "کد ملی یا شماره موبایل تکراری است. به صفحه پیگیری مراجعه بفرمایید");
            else
            {
                token = GenerateJwtTokenForApplicant(regId, firstName, lastName, applicant.Id, nationalCode, mobile, appSetting);
                return ActionReport<TokenDto>.Success(token);
            }


        applicant = new Applicant
        {
            FirstName = firstName,
            LastName = lastName,
            RegId = regId,
            PhoneNumber = mobile,
            NationalNumber = nationalCode,
        };
        ActionReport report = await Add(applicant);
        if (!report.Successful)
        {
            return ActionReport<TokenDto>.Error(report);
        }
        token = GenerateJwtTokenForApplicant(regId, firstName, lastName, applicant.Id, nationalCode, mobile, appSetting);
        return ActionReport<TokenDto>.Success(token);
    }
    public async Task<ActionReport<TokenDto>> SingIn(int regId, string nationalCode, string mobile, string trackingCode, AppSettings appSetting)
    {
        Applicant? applicant = await FirstOrDefaultAsync(e =>
            e.RegId == regId &&
            e.NationalNumber == nationalCode &&
            e.PhoneNumber == mobile &&
            e.TrackingCode == trackingCode);

        if (applicant is null)
            return ActionReport<TokenDto>.Error(HttpStatusCode.NotFound, "اطلاعات شما یافت نشد");

        TokenDto token = GenerateJwtTokenForApplicant(regId, applicant.FirstName, applicant.LastName, applicant.Id, nationalCode, mobile, appSetting);
        return ActionReport<TokenDto>.Success(token);
    }


    private TokenDto GenerateJwtTokenForApplicant(int regId, string firstName, string lastName, int applicantId, string nationalCode, string mobile, AppSettings appSetting)
    {
        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSetting.SecretKey));
        SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        Claim[] claims =
        [
            new(ClaimTypes.Actor, "Applicant"),
            new(ClaimTypes.Name, firstName),
            new(ClaimTypes.GivenName, lastName),
            new(ClaimTypes.SerialNumber, applicantId.ToString()),
            new(ClaimTypes.PrimarySid, regId.ToString()),
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

    public Task<ApplicantDto?> GetStatus(int applicantId)
        =>
            Where(e => e.Id == applicantId)
                .Select(e => new ApplicantDto
                {
                    StatusId = e.StatusId,
                    CreatedDate = e.CreatedDate,
                    RegStepId = e.Status != null ? e.Status.RegStepId : null,
                    Title = e.Status != null ? e.Status.Title : null,
                    IsWaiting = e.Status != null ? e.Status.IsWaiting : null,
                    IsAccepted = e.Status != null ? e.Status.IsAccepted : null,
                    IsNotChecked = e.Status != null ? e.Status.IsNotChecked : null,
                    IsRejected = e.Status != null ? e.Status.IsRejected : null,
                    IsReserved = e.Status != null ? e.Status.IsReserved : null,
                })
                .FirstOrDefaultAsync();

    public Task<List<Applicant>> GetMembers(int applicantId)
        => Where(ent => ent.LeaderId == applicantId)
            .OrderByDescending(e=>e.Id)
            .ToListAsync();

    public async Task<ActionReport<MemberInfoDto>> AddMember(int applicantId, int regStepId, string firstName, string lastName, string nationalCode, string mobile)
    {
        RegStep? regStep = await regStepBusiness.GetById(regStepId);
        byte? limit = regStep!.MemberLimit;
        if (limit > 0)
        {
            Applicant? sameApplicant =
                await FirstOrDefaultAsync(e => e.RegId == regStep.RegId && e.NationalNumber == nationalCode);
            if (sameApplicant is not null)
                return ActionReport<MemberInfoDto>.Error(HttpStatusCode.Conflict);
            var membersCount = await Where(e => e.LeaderId == applicantId).CountAsync();
            if (membersCount >= limit)
                return ActionReport<MemberInfoDto>.Error(HttpStatusCode.InsufficientStorage);
            
            Applicant newMember = new()
            {
                FirstName = firstName,
                LastName = lastName,
                LeaderId = applicantId,
                NationalNumber = nationalCode,
                PhoneNumber = mobile,
                RegId = regStep.RegId,

            };

            var report = await Add(newMember);
            if (report.Successful)
                return ActionReport<MemberInfoDto>.Success(Mapper.Map<MemberInfoDto>(newMember));
        }
        return ActionReport<MemberInfoDto>.Error(HttpStatusCode.Forbidden);
    }
}