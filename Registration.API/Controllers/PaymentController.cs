using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Registration.API.Business;
using Registration.API.Common;

namespace Registration.API.Controllers;
using Payment = Entity.Models.Payment;

public class PaymentController(PaymentBusiness paymentBusiness, ApplicantBusiness applicantBusiness, PaymentBusiness b, IMapper m, IOptions<AppSettings> ap, IHttpContextAccessor ac) :
    GenericController<PaymentBusiness, Payment>(b, m, ap, ac)
{

    [HttpGet("{regStepId}")]
    public async Task<IActionResult> GetAmountByRegStepId(int regStepId)
    {
        Payment? payment = await paymentBusiness.GetByRegStepId(regStepId);
        if (payment is null)
            return NotFound();

        int membersCount = await applicantBusiness.GetMembersCount(ApplicantId);

        return Ok((membersCount + 1) * payment.PerPersonAmount);
    }
}