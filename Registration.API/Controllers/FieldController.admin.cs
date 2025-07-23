using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Registration.API.Entity.Dtos;

namespace Registration.API.Controllers
{
    public partial class FieldController
    {
        [HttpGet("{regId}")]
        [Authorize("Admin")]
        public async Task<IActionResult> GetAll(int regId)
        {
            return Ok(Mapper.Map<List<FieldDto>>(await Business.GetAllWithOptions(regId)));
        }

    }
}
