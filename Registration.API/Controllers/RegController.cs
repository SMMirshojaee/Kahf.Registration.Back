using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Registration.API.Business;
using Registration.API.Common;
using Registration.API.Entity.Dtos;

namespace Registration.API.Controllers;

public class RegController(RegBusiness b, IMapper m, IOptions<AppSettings> ap,IHttpContextAccessor ac) : GenericController<RegBusiness, Reg>(b, m, ap,ac)
{
    //[HttpGet]
    //public async Task<IActionResult> GetAll() => 
    //    Ok(Mapper.Map<List<RegDto>>(await Business.GetAll()));

    //[HttpGet]
    //[AllowAnonymous]
    //public async Task<IActionResult> GetDefault() =>
    //    Ok(Mapper.Map<RegDto>(await Business.GetById(AppSetting.DefaultRegId)));
    
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetActiveRegs() =>
        Ok(Mapper.Map<List<RegDto>>(await Business.GetActiveRegs()));
}