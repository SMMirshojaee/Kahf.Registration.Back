using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Registration.API.Business;
using Registration.API.Common;
using Registration.API.Entity.Dtos;

namespace Registration.API.Controllers;

using Payment = Entity.Models.Payment;

public class PaymentController(
	ApplicantBusiness applicantBusiness,
	PaymentBusiness b,
	IMapper m,
	IOptions<AppSettings> ap,
	IHttpContextAccessor ac,
	ILoggerFactory lg) :
	GenericController<PaymentBusiness, Payment>(b, m, ap, ac, lg)
{
	[HttpGet("{regStepId}")]
	public async Task<IActionResult> GetAmountByRegStepId(int regStepId)
	{
		Payment? payment = await Business.GetByRegStepId(regStepId);
		if (payment is null)
			return NotFound();

		int membersCount = await applicantBusiness.GetMembersCount(ApplicantId);

		return Ok((membersCount + 1) * payment.PerPersonAmount);
	}

	[HttpGet("{regStepId}")]
	public async Task<IActionResult> GetByRegStepId(int regStepId) =>
		Ok<PaymentDto?>(await Business.GetByRegStepId(regStepId));

	[HttpGet("{regId}")]
	public async Task<IActionResult> GetByRegId(int regId) =>
		Ok<List<PaymentDto>>(await Business.Where(e => e.RegStep.RegId == regId).ToListAsync());
}