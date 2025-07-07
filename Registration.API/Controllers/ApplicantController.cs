using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Registration.API.Business;
using Registration.API.Common;
using Registration.API.Entity.Dtos;

namespace Registration.API.Controllers;

public class ApplicantController(ApplicantBusiness business, IMapper mapper, IOptions<AppSettings> appSetting) : GenericController<ApplicantBusiness, Applicant>(business, mapper, appSetting)
{
    [HttpPost]
    public async Task<IActionResult> Signup(SignupDto signupForm)
        => Status(await Business.Signup(signupForm, AppSetting.DefaultRegId));
}