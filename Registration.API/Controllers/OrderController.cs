using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Payment;
using Registration.API.Business;
using Registration.API.Common;
using Registration.API.Entity.Dtos;
using Registration.API.Entity.Models;

namespace Registration.API.Controllers
{
    using Payment = Entity.Models.Payment;
    public class OrderController(SmsHelper smsHelper, Zarrinpal paymentService, RegStepBusiness regStepBusiness, IHostEnvironment environment, ApplicantBusiness applicantBusiness, PaymentBusiness paymentBusiness, OrderBusiness business, IMapper mapper, IOptions<AppSettings> appSetting, IHttpContextAccessor contextAccessor) :
        GenericController<OrderBusiness, Order>(business, mapper, appSetting, contextAccessor)
    {
        private IHostEnvironment env = environment;

        [HttpGet("{regStepId}/{loan}/{cash}")]
        public async Task<IActionResult> RequestLoan(int regStepId, int loan, int cash)
        {
            Payment? payment = await paymentBusiness.GetByRegStepId(regStepId);
            if (payment == null)
                return NotFound();
            int membersCount = await applicantBusiness.GetMembersCount(ApplicantId);

            int totalAmount = (membersCount + 1) * payment.PerPersonAmount;
            if (totalAmount != loan + cash)
                return BadRequest();

            if (loan <= 0 && cash <= 0)
                return StatusCode((int)HttpStatusCode.Forbidden);

            Applicant applicant = (await applicantBusiness.GetById(ApplicantId, true))!;

            if (loan > 0)
            {
                Order loanOrder = new()
                {
                    ApplicantId = ApplicantId,
                    Amount = loan,
                    NationalNumber = NationalCode,
                    RegStepId = regStepId,
                    RequestStatus = 100,
                    VerifyStatus = 100,
                    Authority = "LOAN"
                };
                ActionReport loanReport = await Business.Add(loanOrder);
                if (!loanReport.Successful)
                    return InternalServerError("خطا در ثبت وام");

                await smsHelper.Send(ApplicantId, NationalCode, Mobile,
                    "زائر گرامی درخواست وام شما ثبت شد. لطفا جهت پیگیری مراحل دریافت وام، به آی دی @saber_azad در پیامرسان بله پیام بدهید",
                    null);


                if (payment.LoanStatusId.HasValue)
                {
                    applicant.StatusId = payment.LoanStatusId.Value;
                    ActionReport report = await Business.SaveChanges();
                    if (!report.Successful)
                        return InternalServerError("وام شما ثبت گردید اما وضعیت شما در سامانه ثبت نشد. این مشکل را به مدیر سامانه اطلاع دهید");
                }
                else
                {
                    RegStep regStep = (await regStepBusiness.GetByIdWithStatuses(regStepId))!;

                    RegStepStatus rejectStatus = regStep.RegStepStatuses.First(e => e.IsAccepted);
                    applicant.StatusId = rejectStatus.Id;
                    ActionReport report = await Business.SaveChanges();
                    if (!report.Successful)
                        return InternalServerError("وام شما ثبت گردید اما وضعیت شما در سامانه ثبت نشد. این مشکل را به مدیر سامانه اطلاع دهید");

                }
                if (cash <= 0)
                {

                    return Ok("وام شما با موفقیت ثبت شد. لطفا به پنل کاربری مراجعه کرده و فرایند را کامل نمایید");
                }
            }

            Order cashOrder = new()
            {
                ApplicantId = ApplicantId,
                Amount = cash,
                NationalNumber = NationalCode,
                RegStepId = regStepId,
            };
            ActionReport cashReport = await Business.Add(cashOrder);
            if (!cashReport.Successful)
                return InternalServerError("خطا در ثبت بخش نقدی");

            ZarrinpalResponse requestReport = await paymentService.SendRequest(id: ApplicantId, firstName: applicant.FirstName, lastName: applicant.LastName, amount: cashOrder.Amount, orderId: cashOrder.Id, mobile: Mobile, devMode: env.IsDevelopment());
            if (!requestReport.Successful)
            {
                await Business.UpdateRequest(cashOrder.Id, requestReport.Authority, requestReport.Content, requestReport.Code);
                return InternalServerError("خطا در ارتباط با درگاه بانکی");
            }

            cashReport = await Business.UpdateRequest(cashOrder.Id, requestReport.Authority, requestReport.Content, requestReport.Code);
            return Ok(requestReport.RedirectUrl);
        }

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

            Applicant? applicant = await applicantBusiness.GetById(ApplicantId);

            ZarrinpalResponse requestReport = await paymentService.SendRequest(id: ApplicantId, firstName: applicant.FirstName, lastName: applicant.LastName, amount: newOrder.Amount, orderId: newOrder.Id, mobile: Mobile, devMode: env.IsDevelopment());
            if (!requestReport.Successful)
            {
                await Business.UpdateRequest(newOrder.Id, requestReport.Authority, requestReport.Content, requestReport.Code);
                return InternalServerError("خطا در ارتباط با درگاه بانکی");
            }

            report = await Business.UpdateRequest(newOrder.Id, requestReport.Authority, requestReport.Content, requestReport.Code);
            return Ok(requestReport.RedirectUrl);
        }
        [HttpGet("{regStepId}/{cash}")]
        public async Task<IActionResult> PayCash(int regStepId, int cash)
        {
            Order newOrder = new Order
            {
                ApplicantId = ApplicantId,
                Amount = cash,
                NationalNumber = NationalCode,
                RegStepId = regStepId,
            };
            ActionReport report = await Business.Add(newOrder);
            if (!report.Successful)
                return InternalServerError("خطا در ثبت سفارش");

            Applicant? applicant = await applicantBusiness.GetById(ApplicantId);

            ZarrinpalResponse requestReport = await paymentService.SendRequest(id: ApplicantId, firstName: applicant.FirstName, lastName: applicant.LastName, amount: newOrder.Amount, orderId: newOrder.Id, mobile: Mobile, devMode: env.IsDevelopment());
            if (!requestReport.Successful)
            {
                await Business.UpdateRequest(newOrder.Id, requestReport.Authority, requestReport.Content, requestReport.Code);
                return InternalServerError("خطا در ارتباط با درگاه بانکی");
            }

            report = await Business.UpdateRequest(newOrder.Id, requestReport.Authority, requestReport.Content, requestReport.Code);
            return Ok(requestReport.RedirectUrl);
        }

        [HttpGet("{regStepId}")]
        public async Task<IActionResult> GetPrevious(int regStepId)
            => Ok<List<OrderDto>>(await Business.Where(e => e.ApplicantId == ApplicantId &&
                                                            e.RegStepId == regStepId &&
                                                            e.RequestStatus == 100 &&
                                                            e.VerifyStatus == 100).ToListAsync());

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Callback([FromQuery] string authority, [FromQuery] string status)
        {
            Order? order = await Business.GetByAuthority(authority);
            if (order is null)
                return Redirect($"{AppSetting.FrontPaymentPage}?authority={authority}&messageCode=2");
            ZarrinpalResponse verifyResponse;
            if (!order.RegStepId.HasValue)
            {
                if (status.ToLower() != "ok")
                {
                    return Redirect($"{AppSetting.FrontPaymentPage}?authority={authority}&messageCode=1");
                }
                else
                {
                    verifyResponse = await paymentService.Verify(authority, order.Amount);
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
                    return Redirect($"{AppSetting.FrontPaymentPage}?authority={authority}&messageCode=0&refId={verifyResponse.RefId}");

                }
            }
            RegStep regStep = (await regStepBusiness.GetByIdWithStatuses(order.RegStepId.Value))!;

            RegStepStatus rejectStatus = regStep.RegStepStatuses.First(e => e.IsRejected);

            Applicant applicant = (await applicantBusiness.GetById(order.ApplicantId!.Value, true))!;

            if (status.ToLower() != "ok")
            {
                applicant.StatusId = rejectStatus.Id;
                await Business.SaveChanges();
                return Redirect($"{AppSetting.FrontPaymentPage}?authority={authority}&messageCode=1");
            }

            verifyResponse = await paymentService.Verify(authority, order.Amount);
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

            bool hasLoan = await Business.Where(e => e.RegStepId == regStep.Id &&
                                                     e.ApplicantId == applicant.Id &&
                                                     e.RequestStatus == 100 && e.VerifyStatus == 100 &&
                                                     e.Authority == "LOAN").AnyAsync();
            if (!hasLoan)
                applicant.StatusId = regStep.RegStepStatuses.First(e => e.IsAccepted).Id;
            await Business.SaveChanges();
            return Redirect($"{AppSetting.FrontPaymentPage}?authority={authority}&messageCode=0&refId={verifyResponse.RefId}");
        }
    }
}
