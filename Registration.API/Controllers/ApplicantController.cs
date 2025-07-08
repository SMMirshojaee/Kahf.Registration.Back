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
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Signup(SignupDto signupForm)
        => Status(await Business.Signup(signupForm, AppSetting.DefaultRegId,AppSetting));
}