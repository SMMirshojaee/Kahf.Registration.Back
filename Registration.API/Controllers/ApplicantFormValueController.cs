using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Registration.API.Business;
using Registration.API.Common;
using Registration.API.Entity.Dtos;

namespace Registration.API.Controllers;

public class ApplicantFormValueController(ApplicantFormValueBusiness b, IMapper m, IOptions<AppSettings> ap, IHttpContextAccessor ac)
    : GenericController<ApplicantFormValueBusiness, ApplicantFormValue>(b, m, ap, ac)
{

    [HttpGet("{regStepId}")]
    public async Task<IActionResult> GetByRegStepId(int regStepId) =>
        Ok(Mapper.Map<List<ApplicantFormValueDto>>(
            await Business.GetByApplicantIdAndRegStepId(ApplicantId, regStepId)));
}