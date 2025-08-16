using System.Net;
using System.Reflection;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Registration.API.Business;
using Registration.API.Common;
using Registration.API.Entity.Dtos;
using SMS;

namespace Registration.API.Controllers;

public partial class ApplicantController(ApplicantExtraCostBusiness applicantExtraCostBusiness, SmsHelper smsSender, RegStepStatusBusiness regStepStatusBusiness, ApplicantFormValueBusiness applicantFormValueBusiness, RegStepBusiness regStepBusiness, ApplicantBusiness b, IMapper m, IOptions<AppSettings> ap, IHttpContextAccessor ac) :
    GenericController<ApplicantBusiness, Applicant>(b, m, ap, ac)
{
    [HttpPost("{regId}")]
    [AllowAnonymous]
    public async Task<IActionResult> Signup(int regId, SignupDto signupForm) =>
        Status(await Business.Signup(regId, signupForm.FirstName, signupForm.LastName, signupForm.NationalCode,
            signupForm.Mobile, AppSetting));

    [HttpPut("{regId}")]
    [AllowAnonymous]
    public async Task<IActionResult> SingIn(int regId, SigninDto signinForm)
        => Status(await Business.SingIn(regId, signinForm.NationalCode, signinForm.Mobile, signinForm.TrackingCode, AppSetting));

    [HttpGet]
    public async Task<IActionResult> GetStatus() =>
        Ok(await Business.GetStatus(ApplicantId));

    [HttpGet]
    public async Task<IActionResult> GetMembers() =>
        Ok(Mapper.Map<List<MemberInfoDto>>(await Business.GetMembers(ApplicantId)));

    [HttpGet]
    public async Task<IActionResult> GetMembersCount() =>
        Ok(await Business.GetMembersCount(ApplicantId));

    [HttpPost("{regStepId}")]
    public async Task<IActionResult> AddMember(int regStepId, SignupDto addMemberForm)
    => Status(await Business.AddMember(ApplicantId, regStepId, addMemberForm.FirstName, addMemberForm.LastName, addMemberForm.NationalCode, addMemberForm.Mobile));

    [HttpDelete("{memberId}")]
    public async Task<IActionResult> RemoveMember(int memberId)
    {
        Applicant? member = await Business.GetById(memberId);
        if (member is null)
            return NotFound();
        if (member.LeaderId != ApplicantId)
            return Unauthorized();

        ActionReport report = await Business.RemoveMember(memberId);
        return Status(report);
    }

    [HttpPut("{regStepId}")]
    public async Task<IActionResult> FinishFormStep(int regStepId)
    {
        Applicant? applicant = await Business.GetById(ApplicantId, true);
        if (applicant is null) return NotFound();
        RegStep? regStep = await regStepBusiness.GetByIdWithStatuses(regStepId);
        if (regStep is null) return NotFound();
        bool hasAnyAnswer = await applicantFormValueBusiness.Where(e => e.ApplicantId == ApplicantId
                                                                           && !e.Deleted).AnyAsync();

        if (!hasAnyAnswer)
            return StatusCode((int)HttpStatusCode.Forbidden);

        RegStepStatus? notCheckedStatus = regStep.RegStepStatuses.FirstOrDefault(e => e.IsNotChecked);
        if (notCheckedStatus is null) return NotFound();
        applicant.StatusId = notCheckedStatus.Id;
        return Status(await Business.SaveChanges());
    }

    [HttpGet]
    public async Task<IActionResult> GetExtraCosts()
        => Ok<List<ApplicantExtraCostDto>>(await applicantExtraCostBusiness.GetByApplicantId(ApplicantId));

}