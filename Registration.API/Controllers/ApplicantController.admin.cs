using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Registration.API.Common;
using Registration.API.Entity.Dtos;

namespace Registration.API.Controllers
{
    public partial class ApplicantController
    {

        [HttpGet("{regId}")]
        [Authorize("Admin")]
        public async Task<IActionResult> GetByRegId(int regId) =>
            Ok(Mapper.Map<List<ApplicantInfoDto>>(await Business.GetByRegId(regId)));

        [HttpGet("{regStepId}")]
        [Authorize("Admin")]
        public async Task<IActionResult> GetLeadersWithFormValuesAndMembersWithRegStepId(int regStepId) =>
            Ok(Mapper.Map<List<ApplicantWithFormValueDto>>(await Business.GetLeadersWithFormValuesAndMembersWithRegStepId(regStepId)));

        [HttpGet("{regId}")]
        [Authorize("Admin")]
        public async Task<IActionResult> GetLeadersFullDataByRegId(int regId) =>
            Ok(Mapper.Map<List<ApplicantWithFormValueDto>>(await Business.GetLeadersFullDataByRegId(regId)));


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
                ActionReport sendSmsReport = await smsSender.Send(applicant.Id, applicant.NationalNumber, applicant.PhoneNumber, smsText, UserId);
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
        public async Task<IActionResult> TransferToNextStep(int regStepId, int nextStatusId, bool sendSms, [FromBody] string? smsText, [FromQuery] List<int> ids)
        {
            ActionReport<List<Applicant>> report = await Business.TransferAccepted(regStepId, ids, nextStatusId);
            if (!report.Successful)
                return Status(report);
            if (report.Output is null || !report.Output.Any())
                return BadRequest("هیچ زائر تایید شده ای وجود ندارد");
            if (sendSms && !string.IsNullOrEmpty(smsText))
                foreach (Applicant applicant in report.Output)
                    await smsSender.Send(applicant.Id, applicant.NationalNumber, applicant.PhoneNumber, smsText, UserId);

            return Ok(report.Output.Count);
        }

        [HttpGet("{regId}")]
        [Authorize("Admin")]
        public async Task<IActionResult> GetWithOrders(int regId)
            => Ok(await Business.GetWithOrders(regId));

        [HttpDelete("{id}")]
        [Authorize("SuperAdmin")]
        public async Task<IActionResult> RemoveExtraCost(int id)
            => Status(await applicantExtraCostBusiness.Delete(id));

        [HttpPost]
        [Authorize("SuperAdmin")]
        public async Task<IActionResult> InsertInstallment([FromQuery] DateTime date, [FromBody] InstallmentDto newInstallment)
        {
            newInstallment.Date = date;
            ActionReport<OrderDto> report = await orderBusiness.InsertInstallment(UserId, newInstallment);
            return Status(report);
        }
        
        [HttpDelete("{id}")]
        [Authorize("SuperAdmin")]
        public async Task<IActionResult> RemoveInstallment(int id)
        {
            return Status(await orderBusiness.Delete(id));
        }
    }
}
