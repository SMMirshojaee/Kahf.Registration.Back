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
	ILoggerFactory lg,
	SmsHelper smsHelper) : GenericController<RegBusiness, Reg>(b, m, ap, ac, lg)
{
	[HttpGet]
	[AllowAnonymous]
	public async Task<IActionResult> TEST() =>
		Ok(await smsHelper.Send(477, "0011227168", "09128486146", "سلام", null));

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