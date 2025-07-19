using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Payment;
using Registration.API.Business;
using Registration.API.Common;

namespace Registration.API.Controllers
{
    using Payment = Entity.Models.Payment;
    public class OrderController(IHostEnvironment env, ApplicantBusiness applicantBusiness, PaymentBusiness paymentBusiness, OrderBusiness business, IMapper mapper, IOptions<AppSettings> appSetting, IHttpContextAccessor contextAccessor) :
        GenericController<OrderBusiness, Order>(business, mapper, appSetting, contextAccessor)
    {

        [HttpGet("{regStepId}")]
        public async Task<IActionResult> SendRequest(int regStepId)
        {
            Payment? payment = await paymentBusiness.GetByRegStepId(regStepId);
            if (payment == null)
                return NotFound();
            int membersCount = await applicantBusiness.GetMembersCount(ApplicantId);
            Order newOrder = new Order
            {
                ApplicantId = ApplicantId,
                Amount = (membersCount + 1) * payment.PerPersonAmount,
                NationalNumber = NationalCode,
                RegStepId = regStepId,
            };
            ActionReport report = await Business.Add(newOrder);
            if (!report.Successful)
                return InternalServerError("خطا در ثبت سفارش");

            Zarrinpal zarrinpal = new(env.IsDevelopment());
            ZarrinpalResponse requestReport = await zarrinpal.SendRequest(amount: newOrder.Amount, newOrder.Id, Mobile);
            if (!requestReport.Successful)
            {
                await Business.UpdateRequest(newOrder.Id, requestReport.Authority, requestReport.Content, requestReport.Code);
                return InternalServerError("خطا در ارتباط با درگاه بانکی");
            }

            report = await Business.UpdateRequest(newOrder.Id, requestReport.Authority, requestReport.Content, requestReport.Code);
            return Ok(requestReport.RedirectUrl);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Callback([FromQuery] string authority, [FromQuery] string status)
        {
            if (status.ToLower() != "ok")
                return Redirect($"{AppSetting.FrontPaymentPage}?authority={authority}&messageCode=1");
            Order? order = await Business.GetByAuthority(authority);
            if (order is null)
                return Redirect($"{AppSetting.FrontPaymentPage}?authority={authority}&messageCode=2");

            Zarrinpal zarrinpal = new(env.IsDevelopment());
            ZarrinpalResponse verifyResponse = await zarrinpal.Verify(authority, order.Amount);
            if (!verifyResponse.Successful)
            {
                order.VerifyContent = verifyResponse.Content;
                order.VerifyStatus = verifyResponse.Code;
                order.VerifyDate = DateTime.Now;
                await Business.SaveChanges();
                return Redirect($"{AppSetting.FrontPaymentPage}?authority={authority}&messageCode=3");
            }
            order.VerifyContent = verifyResponse.Content;
            order.VerifyStatus = verifyResponse.Code;
            order.RefId = verifyResponse.RefId;
            order.VerifyDate = DateTime.Now;
            await Business.SaveChanges();
            return Redirect($"{AppSetting.FrontPaymentPage}?authority={authority}&messageCode=0&refId=${verifyResponse.RefId}");
        }
    }
}
