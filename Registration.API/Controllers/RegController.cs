using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Registration.API.Business;
using Registration.API.Common;
using Registration.API.Entity.Dtos;

namespace Registration.API.Controllers;

public class RegController(RegBusiness business, IMapper mapper, IOptions<AppSettings> appSettings) : GenericController<RegBusiness, Reg>(business, mapper, appSettings)
{
    //[HttpGet]
    //public async Task<IActionResult> GetAll() => 
    //    Ok(Mapper.Map<List<RegDto>>(await Business.GetAll()));

    [HttpGet]
    public async Task<IActionResult> GetDefault() =>
        Ok(Mapper.Map<RegDto>(await Business.GetById(AppSetting.DefaultRegId)));
}