using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Registration.API.Business;
using Registration.API.Common;
using Registration.API.Entity.Dtos;

namespace Registration.API.Controllers
{
    public class FieldController(FieldBusiness business, IMapper mapper, IOptions<AppSettings> appSettings) : GenericController<FieldBusiness, Field>(business, mapper, appSettings)
    {
        [HttpGet("regStepId")]
        public async Task<IActionResult> GetByRegStepId(int regStepId)
            => Ok(Mapper.Map<List<FieldDto>>(await Business.GetByRegStepId(regStepId)));
    }
}
