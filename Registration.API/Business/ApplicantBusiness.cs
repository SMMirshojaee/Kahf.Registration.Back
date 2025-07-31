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
using Registration.API.Entity.Models;

namespace Registration.API.Business;

public class ApplicantBusiness(RegStepBusiness regStepBusiness, RegContext context, IMapper mapper) : GenericBusiness<Applicant>(context, mapper)
{
    public async Task<ActionReport<TokenDto>> Signup(int regId, string firstName, string lastName, string nationalCode, string mobile, AppSettings appSetting)
    {
        Applicant? applicant = await FirstOrDefaultAsync(e =>
            e.RegId == regId && (e.NationalNumber == nationalCode || e.PhoneNumber == mobile));
        TokenDto token;

        if (applicant is not null)
            if (applicant.NationalNumber == nationalCode && applicant.PhoneNumber == mobile && string.IsNullOrEmpty(applicant.TrackingCode))
            {
                token = GenerateJwtTokenForApplicant(regId, applicant.FirstName, applicant.LastName, applicant.Id,
                    nationalCode, mobile, appSetting);
                return ActionReport<TokenDto>.Success(token);
            }
            else
                return ActionReport<TokenDto>.Error(HttpStatusCode.Conflict, "کد ملی یا شماره موبایل وارد شده قبلا ثبت شده است.لطفا به صفحه پیگیری بروید");

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
            !e.LeaderId.HasValue &&
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
            .OrderByDescending(e => e.Id)
            .ToListAsync();

    public Task<int> GetMembersCount(int applicantId)
        => Where(ent => ent.LeaderId == applicantId)
            .CountAsync();

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
            int membersCount = await Where(e => e.LeaderId == applicantId).CountAsync();
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

            ActionReport report = await Add(newMember);
            if (report.Successful)
                return ActionReport<MemberInfoDto>.Success(Mapper.Map<MemberInfoDto>(newMember));
        }
        return ActionReport<MemberInfoDto>.Error(HttpStatusCode.Forbidden);
    }

    public async Task<ActionReport> RemoveMember(int memberId)
    {
        Applicant? member = await FirstOrDefaultAsync(e => e.Id == memberId);
        if (member is null)
            return ActionReport.Error(HttpStatusCode.NotFound);
        ActionReport report = await Delete(memberId);
        return report;
    }

    public Task<List<Applicant>> GetByRegId(int regId)
    {
        return Where(e => e.RegId == regId)
            .Include(e => e.InverseLeader)
        .ToListAsync();
    }

    public async Task<List<Applicant>> GetLeadersWithFormValuesAndMembersWithRegStepId(int regStepId)
    {
        RegStep? regStep = await regStepBusiness.Where(e => e.Id == regStepId)
            .Include(e => e.RegStepStatuses)
            .FirstOrDefaultAsync();
        if (regStep is null)
            return new List<Applicant>();
        return await Where(e => !e.LeaderId.HasValue && ((regStep.Order == 1 && !e.StatusId.HasValue && e.RegId == regStep.RegId) || e.Status.RegStepId == regStepId))
            .Include(e => e.ApplicantFormValues)
            .Include(e => e.InverseLeader).ThenInclude(e => e.ApplicantFormValues)
            .Include(e => e.Status)
            .Include(e => e.Messages)
            .ToListAsync();
    }

    public async Task<ActionReport> UpdateDescription(int applicantId, string? description)
    {
        Applicant? applicant = await GetById(applicantId, true);
        if (applicant is null)
            return ActionReport.Error(HttpStatusCode.NotFound);
        applicant.Description = description;
        return await SaveChanges();
    }

    public async Task<ActionReport<List<Applicant>>> TransferAccepted(int regStepId, int nextStatusId)
    {
        RegStep? regStep = await regStepBusiness.GetByIdWithStatuses(regStepId);
        if (regStep is null)
            return ActionReport<List<Applicant>>.Error(HttpStatusCode.NotFound);
        int acceptedStatusId = regStep.RegStepStatuses.First(e => e.IsAccepted).Id;
        List<Applicant> acceptedApplicants = await Where(e => e.StatusId == acceptedStatusId, true).ToListAsync();
        if (!acceptedApplicants.Any())
            return ActionReport<List<Applicant>>.Success(new());
        foreach (Applicant applicant in acceptedApplicants)
        {
            applicant.StatusId = nextStatusId;
        }
        var report = await SaveChanges();
        if (report.Successful)
            return ActionReport<List<Applicant>>.Success(acceptedApplicants);
        return ActionReport<List<Applicant>>.Error(report);
    }

    public Task<List<Applicant>> GetLeadersFullDataByRegId(int regId)
        => Where(e => !e.LeaderId.HasValue && e.RegId == regId)
            .Include(e => e.ApplicantFormValues)
            .Include(e => e.InverseLeader).ThenInclude(e => e.ApplicantFormValues)
            .Include(e => e.Status)
            .Include(e => e.Messages)
            .ToListAsync();

    public Task<List<ApplicantOrderDto>> GetWithOrders(int regId) =>
        Where(e => e.RegId == regId && e.StatusId.HasValue && !e.LeaderId.HasValue)
            .Select(e => new ApplicantOrderDto
            {
                Id = e.Id,
                FirstName = e.FirstName,
                LastName = e.LastName,
                NationalNumber = e.NationalNumber,
                PhoneNumber = e.PhoneNumber,
                StatusId = e.StatusId,
                StatusTitle = e.Status != null ? e.Status.Title : string.Empty,
                StepTitle = e.Status != null ? e.Status.RegStep.Title : string.Empty,
                MembersCount = e.InverseLeader.Count,
                Orders = Mapper.Map<List<OrderDto>>(e.Orders.Where(o => o.RequestStatus == 100 && o.VerifyStatus == 100))
            })
            .ToListAsync();

    public Task<List<Applicant>> GetByIds(List<int> ids)
        => Where(e => ids.Contains(e.Id)).ToListAsync();
}