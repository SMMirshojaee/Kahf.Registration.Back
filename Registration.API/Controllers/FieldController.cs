using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Registration.API.Business;
using Registration.API.Common;
using Registration.API.Entity.Dtos;

namespace Registration.API.Controllers
{
    public class FieldController(FieldBusiness b, IMapper m, IOptions<AppSettings> ap, IHttpContextAccessor ac) : GenericController<FieldBusiness, Field>(b, m, ap, ac)
    {
        [HttpGet("{regStepId}/{memberId?}")]
        public async Task<IActionResult> GetByRegStepId(int regStepId, int? memberId = null)
            => Ok(Mapper.Map<List<FieldDto>>(await Business.GetByRegStepId(regStepId, ApplicantId, memberId)));
    }
}
