using Microsoft.AspNetCore.Mvc;
using Registration.API.Business;

namespace Registration.API.Controllers;

public class RegController(RegBusiness business) : GenericController<RegBusiness, Reg>(business)
{

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await Business.GetAll());
}