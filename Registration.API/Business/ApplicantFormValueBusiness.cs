using System.Net;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Registration.API.Common;
using Registration.API.Entity.Dtos;

namespace Registration.API.Business;

public class ApplicantFormValueBusiness(ApplicantBusiness applicantBusiness, RegContext context, IMapper mapper) : GenericBusiness<ApplicantFormValue>(context, mapper)
{
    public async Task<List<ApplicantFormValue>> GetByApplicantIdAndRegStepId(int applicantId, int regStepId, int? memberId)
    {
        if (memberId.HasValue)
        {
            Applicant? member = await applicantBusiness.FirstOrDefaultAsync(e => e.Id == memberId);
            if (member is null || member.LeaderId != applicantId)
                return new List<ApplicantFormValue>();
            return await Where(e => e.ApplicantId == memberId && e.RegStepId == regStepId)
                .Include(e => e.Field)
                .Include(e => e.FieldOption)
                .ToListAsync();
        }

        return await Where(e => e.ApplicantId == applicantId && e.RegStepId == regStepId)
            .Include(e => e.Field)
            .Include(e => e.FieldOption)
            .ToListAsync();
    }

    public async Task<bool> HasAccess(int applicantId, int regStepId)
    {
        //TODO: implement
        return true;
    }

    public async Task<ActionReport> Insert(int applicantId, int regStepId, List<ApplicantFormValueDto> values)
    {
        //var previousValues = await Where(e => e.ApplicantId == applicantId &&
        //                                      e.RegStepId == regStepId && e.Deleted == false).ToListAsync();
        //if (hasSomeValues)
        //    return ActionReport.Error(HttpStatusCode.Ambiguous);
        try
        {

            await context.Database.ExecuteSqlInterpolatedAsync(
                $"UPDATE applicant.ApplicantFormValues SET Deleted=1 WHERE ApplicantId={applicantId} AND RegStepId={regStepId}");

            values.ForEach(e =>
            {
                e.ApplicantId = applicantId;
                e.RegStepId = regStepId;
            });

            List<ApplicantFormValue>? valuesToInsert = Mapper.Map<List<ApplicantFormValue>>(values);
            return await Add(valuesToInsert);
        }
        catch (Exception e)
        {
            return ActionReport.Error(HttpStatusCode.InternalServerError, e.Message, e);
        }
    }
}