using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Registration.API.Entity.Dtos;

namespace Registration.API.Controllers
{
    public partial class RegStepController
    {

        [HttpGet("{regId}")]
        [Authorize("Admin")]
        public async Task<IActionResult> GetByRegId(int regId)
        {
            return Ok(Mapper.Map<List<RegStepDto>>(await Business.GetByRegId(regId)));
        }

        [HttpGet("{regStepId}")]
        [Authorize("Admin")]
        public async Task<IActionResult> GetById(int regStepId)
        {
            return Ok(Mapper.Map<RegStepDto>(await Business.GetByIdWithStatuses(regStepId)));
        }
    }
}
