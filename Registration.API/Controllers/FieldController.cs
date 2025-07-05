using Microsoft.AspNetCore.Mvc;
using Registration.API.Business;

namespace Registration.API.Controllers
{
    public class FieldController(FieldBusiness business) : GenericController<FieldBusiness, Field>(business)
    {
        [HttpGet("regStepId")]
        public async Task<IActionResult> GetByRegStepId(int regStepId)
            => Ok(await Business.GetByRegStepId(regStepId));
    }
}
