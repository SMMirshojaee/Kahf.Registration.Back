using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Registration.API.Business;
using Registration.API.Common;
using Registration.API.Entity.Dtos;

namespace Registration.API.Controllers
{
	public partial class RegStepController(
		RegStepBusiness b,
		IMapper m,
		IOptions<AppSettings> ap,
		IHttpContextAccessor ac,
		ILoggerFactory lg) :
		GenericController<RegStepBusiness, RegStep>(b, m, ap, ac, lg)
	{
		[HttpGet]
		public async Task<IActionResult> GetAll() =>
			Ok(Mapper.Map<List<RegStepDto>>(await Business.GetByRegId(RegId)));
	}
}