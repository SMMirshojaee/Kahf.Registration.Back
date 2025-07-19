using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Registration.API.Business;
using Registration.API.Entity.Dtos;

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

        [HttpGet("{applicantId}/{statusId}")]
        [Authorize("Admin")]
        public async Task<IActionResult> ChangeApplicantStatus(int applicantId, int statusId)
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
            var report = await Business.SaveChanges();
            return Status(report);
        }

        [HttpGet("{applicantId}")]
        [Authorize("Admin")]
        public async Task<IActionResult> SaveDescription(int applicantId, [FromQuery] string? description)
        => Status(await Business.UpdateDescription(applicantId, description));
    }
}
