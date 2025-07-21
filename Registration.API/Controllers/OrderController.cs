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
    public class OrderController(Zarrinpal paymentService, RegStepBusiness regStepBusiness, IHostEnvironment environment, ApplicantBusiness applicantBusiness, PaymentBusiness paymentBusiness, OrderBusiness business, IMapper mapper, IOptions<AppSettings> appSetting, IHttpContextAccessor contextAccessor) :
        GenericController<OrderBusiness, Order>(business, mapper, appSetting, contextAccessor)
    {
        private IHostEnvironment env = environment;
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

            var applicant = await applicantBusiness.GetById(ApplicantId);

            ZarrinpalResponse requestReport = await paymentService.SendRequest(id: ApplicantId, firstName: applicant.FirstName, lastName: applicant.LastName, amount: newOrder.Amount, orderId: newOrder.Id, mobile: Mobile);
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
            Order? order = await Business.GetByAuthority(authority);
            if (order is null)
                return Redirect($"{AppSetting.FrontPaymentPage}?authority={authority}&messageCode=2");

            RegStep regStep = (await regStepBusiness.GetByIdWithStatuses(order.RegStepId))!;

            var rejectStatus = regStep.RegStepStatuses.First(e => e.IsRejected);

            Applicant applicant = (await applicantBusiness.GetById(order.ApplicantId, true))!;

            if (status.ToLower() != "ok")
            {
                applicant.StatusId = rejectStatus.Id;
                await Business.SaveChanges();
                return Redirect($"{AppSetting.FrontPaymentPage}?authority={authority}&messageCode=1");
            }

            ZarrinpalResponse verifyResponse = await paymentService.Verify(authority, order.Amount);
            if (!verifyResponse.Successful)
            {
                order.VerifyContent = verifyResponse.Content;
                order.VerifyStatus = verifyResponse.Code;
                order.VerifyDate = DateTime.Now;

                applicant.StatusId = rejectStatus.Id;
                await Business.SaveChanges();
                return Redirect($"{AppSetting.FrontPaymentPage}?authority={authority}&messageCode=3");
            }
            order.VerifyContent = verifyResponse.Content;
            order.VerifyStatus = verifyResponse.Code;
            order.RefId = verifyResponse.RefId;
            order.VerifyDate = DateTime.Now;
            applicant.StatusId = regStep.RegStepStatuses.First(e => e.IsAccepted).Id;
            await Business.SaveChanges();
            return Redirect($"{AppSetting.FrontPaymentPage}?authority={authority}&messageCode=0&refId={verifyResponse.RefId}");
        }
    }
}
