using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Registration.API.Business;
using Registration.API.Common;
using Registration.API.Entity.Dtos;

namespace Registration.API.Controllers;

public class ApplicantFormValueController(ApplicantFormValueBusiness b, IMapper m, IOptions<AppSettings> ap, IHttpContextAccessor ac,
    RegStepBusiness regStepBusiness, ApplicantBusiness applicantBusiness)
    : GenericController<ApplicantFormValueBusiness, ApplicantFormValue>(b, m, ap, ac)
{


    [HttpGet("{regStepId}")]
    public async Task<IActionResult> GetByRegStepId(int regStepId) =>
        Ok(Mapper.Map<List<ApplicantFormValueDto>>(
            await Business.GetByApplicantIdAndRegStepId(ApplicantId, regStepId)));

    [HttpPost("{applicantId}/{regStepId}")]
    public async Task<IActionResult> Insert(int applicantId, int regStepId,
        [FromBody] List<ApplicantFormValueDto> values)
    {
        if (applicantId != ApplicantId)
            return Forbid();
        if (!await Business.HasAccess(applicantId, regStepId))
            return Unauthorized();
        ActionReport report = await Business.Insert(applicantId, regStepId, values);
        if (report.Successful)
        {
            RegStep? regStep = await regStepBusiness.GetById(regStepId);
            Applicant? applicant = await applicantBusiness.GetById(ApplicantId, true);
            if (regStep?.CreateTrackingCode == true && string.IsNullOrEmpty(applicant?.TrackingCode))
            {
                applicant.TrackingCode = CreateRandomString(5, true);
                report = await applicantBusiness.SaveChanges();
                if (report.Successful)
                    return Ok(applicant.TrackingCode);
            }
        }
        return Status(report);
    }

    [HttpPost("{fileName}")]
    public async Task<IActionResult> Upload(string fileName, [FromForm] IFormFile image)
    {
        if (image == null || image.Length == 0)
            return BadRequest("تصویری ارسال نشده");
        try
        {
            string directory = Path.Combine(Directory.GetCurrentDirectory(), AppSetting.RepositoryAddress, ApplicantId.ToString());
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