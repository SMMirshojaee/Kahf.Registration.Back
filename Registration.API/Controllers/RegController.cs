using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Registration.API.Business;
using Registration.API.Common;
using Registration.API.Entity.Dtos;

namespace Registration.API.Controllers;

public class RegController(
	RegBusiness b,
	IMapper m,
	IOptions<AppSettings> ap,
	IHttpContextAccessor ac,
	ILoggerFactory lg) : GenericController<RegBusiness, Reg>(b, m, ap, ac, lg)
{
	[HttpGet]
	[Authorize("Admin")]
	public async Task<IActionResult> GetAll() =>
		Ok(Mapper.Map<List<RegDto>>(await Business.GetAll()));

	[HttpGet("{regId}")]
	[AllowAnonymous]
	public async Task<IActionResult> GetById(int regId)
	{
		RegDto? reg = Mapper.Map<RegDto>(await Business.GetById(regId));
		if (reg is null)
			return NotFound();
		return Ok(reg);
	}

	[HttpGet]
	[AllowAnonymous]
	public async Task<IActionResult> GetActiveRegs() 
		=> Ok(await Business.GetActiveRegs());
}