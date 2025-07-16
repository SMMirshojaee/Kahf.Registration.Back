using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Registration.API.Business;
using Registration.API.Common;
using Registration.API.Entity.Dtos;

namespace Registration.API.Controllers;

public partial class ApplicantFormValueController(ApplicantFormValueBusiness b, IMapper m, IOptions<AppSettings> ap, IHttpContextAccessor ac,
    RegStepBusiness regStepBusiness, ApplicantBusiness applicantBusiness)
    : GenericController<ApplicantFormValueBusiness, ApplicantFormValue>(b, m, ap, ac)
{


    [HttpGet("{regStepId}/{memberId?}")]
    public async Task<IActionResult> GetByRegStepId(int regStepId, int? memberId = null) =>
        Ok(Mapper.Map<List<ApplicantFormValueDto>>(
            await Business.GetByApplicantIdAndRegStepId(ApplicantId, regStepId, memberId)));

    [HttpPost("{regStepId}/{memberId?}")]
    public async Task<IActionResult> Insert(int regStepId,
        [FromBody] List<ApplicantFormValueDto> values, [FromRoute] int? memberId = null)
    {
        if (!await Business.HasAccess(ApplicantId, regStepId))
            return Unauthorized();

        int realApplicantId = ApplicantId;

        if (memberId is not null)
        {
            Applicant? member = await applicantBusiness.GetById(memberId.Value);
            if (member is null || member.LeaderId != ApplicantId)
                return Unauthorized();

            realApplicantId = memberId.Value;

            values.ForEach(value => value.ApplicantId = memberId.Value);
        }

        ActionReport report = await Business.Insert(realApplicantId, regStepId, values);
        if (report.Successful)
        {
            RegStep? regStep = await regStepBusiness.GetByIdWithStatuses(regStepId);
            Applicant? applicant = await applicantBusiness.GetById(realApplicantId, true);
            if (regStep?.CreateTrackingCode == true && !memberId.HasValue && string.IsNullOrEmpty(applicant?.TrackingCode))
            {
                applicant.TrackingCode = CreateRandomString(5, true);
            }

            RegStepStatus? startStatus;
            if (regStep?.MemberLimit > 0)
                startStatus = regStep?.RegStepStatuses.FirstOrDefault(e => e.IsWaiting);
            else
                startStatus = regStep?.RegStepStatuses.FirstOrDefault(e => e.IsNotChecked);
            applicant.StatusId = startStatus?.Id;
            report = await applicantBusiness.SaveChanges();
            if (report.Successful)
                return Ok(applicant.TrackingCode);
        }
        return Status(report);
    }

    [HttpPost("{fileName}/{memberId?}")]
    public async Task<IActionResult> Upload(string fileName, [FromForm] IFormFile image, [FromRoute] int? memberId = null)
    {
        if (image == null || image.Length == 0)
            return BadRequest("تصویری ارسال نشده");
        try
        {
            int realApplicantId = ApplicantId;
            if (memberId is not null)
            {
                Applicant? member = await applicantBusiness.GetById(memberId.Value);
                if (member is null || member.LeaderId != ApplicantId)
                    return Unauthorized();

                realApplicantId = memberId.Value;

            }
            string directory = Path.Combine(Directory.GetCurrentDirectory(), AppSetting.RepositoryAddress, realApplicantId.ToString());
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            string filePath = Path.Combine(directory, fileName);

            await using FileStream stream = new FileStream(filePath, FileMode.Create);
            await image.CopyToAsync(stream);

            return Ok();
        }
        catch (Exception e)
        {
            return InternalServerError();
        }
    }
}