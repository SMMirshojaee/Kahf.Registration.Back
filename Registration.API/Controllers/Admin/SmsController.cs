using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Registration.API.Business;
using Registration.API.Common;
using Registration.API.Entity.Dtos;

namespace Registration.API.Controllers.Admin
{
    public class SmsController(
        SmsHelper smsHelper,
        ApplicantBusiness applicantBusiness,
        MessageBusiness business,
        IMapper mapper,
        IOptions<AppSettings> appSetting,
        IHttpContextAccessor contextAccessor)
        : AdminGenericController<MessageBusiness, Message>(business, mapper, appSetting, contextAccessor)
    {
        [HttpPost]
        public async Task<IActionResult> Send([FromBody] SmsDto smsDto)
        {
            List<Applicant> applicants = await applicantBusiness.GetByIds(smsDto.ApplicantIds);
            ActionReport report = await smsHelper.Send(applicants, smsDto.Text, UserId);
            return Status(report);
        }
    }


}
