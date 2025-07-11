using System.Reflection;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Registration.API.Business;
using Registration.API.Common;
using Registration.API.Entity.Dtos;

namespace Registration.API.Controllers;

public class ApplicantController(ApplicantBusiness b, IMapper m, IOptions<AppSettings> ap, IHttpContextAccessor ac) : GenericController<ApplicantBusiness, Applicant>(b, m, ap, ac)
{
    [HttpPost("{regId}")]
    [AllowAnonymous]
    public async Task<IActionResult> Signup(int regId, SignupDto signupForm)
        => Status(await Business.Signup(regId, signupForm.FirstName, signupForm.LastName, signupForm.NationalCode, signupForm.Mobile, AppSetting));

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

    [HttpPost("{regStepId}")]
    public async Task<IActionResult> AddMember(int regStepId, SignupDto addMemberForm)
    => Status(await Business.AddMember(ApplicantId, regStepId, addMemberForm.FirstName, addMemberForm.LastName, addMemberForm.NationalCode, addMemberForm.Mobile));

}