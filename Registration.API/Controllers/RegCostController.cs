using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Registration.API.Business;
using Registration.API.Common;
using Registration.API.Entity.Dtos;

namespace Registration.API.Controllers;

public class RegCostController(
    RegCostBusiness business,
    IMapper mapper,
    IOptions<AppSettings> appSetting,
    IHttpContextAccessor contextAccessor)
    : GenericController<RegCostBusiness, RegCost>(business, mapper, appSetting, contextAccessor)
{
    [HttpGet("{regId}")]
    public async Task<IActionResult> GetByRegId(int regId)
        => Ok<List<RegCostDto>>(await Business.Where(e => e.RegId == regId).ToListAsync());

}