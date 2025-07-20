using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Registration.API.Business;
using Registration.API.Common;
using Registration.API.Entity.Dtos;
using SMS;

namespace Registration.API.Controllers
{
    public partial class ApplicantController
    {

        [HttpGet("{regId}")]
        [Authorize("Admin")]
        public async Task<IActionResult> GetByRegId(int regId)
        {
            return Ok(Mapper.Map<List<ApplicantInfoDto>>(await Business.GetByRegId(regId)));
        }
        [HttpGet("{regStepId}")]
        [Authorize("Admin")]
        public async Task<IActionResult> GetWithFormValuesWithRegStepId(int regStepId)
        {
            return Ok(Mapper.Map<List<ApplicantWithFormValueDto>>(await Business.GetWithFormValuesWithRegStepId(regStepId)));
        }

        [HttpPut("{applicantId}/{statusId}/{sendSms}")]
        [Authorize("Admin")]
        public async Task<IActionResult> ChangeApplicantStatus(int applicantId, int statusId, bool sendSms, [FromBody] string? smsText)
        {
            Applicant? applicant = await Business.GetById(applicantId, true);
            if (applicant == null)
                return NotFound();

            if (applicant.LeaderId.HasValue)
                return StatusCode((int)HttpStatusCode.Forbidden, "امکان تغییر وضعیت همراه وجود ندارد");

            List<RegStepStatusDto>? sameStepStatuses = Mapper.Map<List<RegStepStatusDto>>(await regStepStatusBusiness.GetSameStepStatuses(statusId));
            if (sameStepStatuses is null || sameStepStatuses.Count == 0)
                return BadRequest("وضعیت انتخاب شده وجود ندارد");

            if (!sameStepStatuses.Select(e => e.Id).Contains(statusId))
                return Forbid("امکان تغییر وضعیت بین مرحله ای وجود ندارد");

            applicant.StatusId = statusId;
            ActionReport report = await Business.SaveChanges();
            if (report.Successful && sendSms && !string.IsNullOrEmpty(smsText))
            {
                Response? sendSmsReport = await smsSender.Send(smsText, applicant.PhoneNumber);
                //TODO
            }
            return Status(report);
        }

        [HttpGet("{applicantId}")]
        [Authorize("Admin")]
        public async Task<IActionResult> SaveDescription(int applicantId, [FromQuery] string? description)
        => Status(await Business.UpdateDescription(applicantId, description));

        [HttpPut("{regStepId}/{nextStatusId}/{sendSms}")]
        [Authorize("Admin")]
        public async Task<IActionResult> TransferToNextStep(int regStepId, int nextStatusId, bool sendSms, [FromBody] string? smsText)
        {
            ActionReport<List<Applicant>> report = await Business.TransferAccepted(regStepId, nextStatusId);
            if (!report.Successful)
                return Status(report);
            if (report.Output is null || !report.Output.Any())
            {
                return BadRequest("هیچ زائر تایید شده ای وجود ندارد");
            }
            if (sendSms && !string.IsNullOrEmpty(smsText))
            {
                await smsSender.Send(smsText, report.Output.Select(e => e.PhoneNumber).ToArray());
            }

            return Ok();
        }
    }
}
