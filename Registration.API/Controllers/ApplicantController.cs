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
        => Status(await Business.Signup(regId, signupForm.NationalCode, signupForm.Mobile, AppSetting));

    [HttpPut("{regId}")]
    [AllowAnonymous]
    public async Task<IActionResult> SingIn(int regId, SigninDto signinForm)
        => Status(await Business.SingIn(regId, signinForm.NationalCode, signinForm.Mobile, signinForm.TrackingCode, AppSetting));
}