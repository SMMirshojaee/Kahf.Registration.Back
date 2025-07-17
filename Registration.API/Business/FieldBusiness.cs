using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Registration.API.Entity.Models;

namespace Registration.API.Business;

public class FieldBusiness(ApplicantBusiness applicantBusiness, RegContext context, IMapper mapper) : GenericBusiness<Field>(context, mapper)
{
    public async Task<List<Field>> GetByRegStepId(int regStepId, int applicantId, int? memberId)
    {
        if (memberId.HasValue)
        {
            Applicant? member = await applicantBusiness.FirstOrDefaultAsync(e => e.Id == memberId);
            if (member is null || member.LeaderId != applicantId)
                return new List<Field>();
        }
        if (memberId is null)
            return await Where(e => e.RegStepId == regStepId && e.ForLeader)
                .Include(e => e.FieldType)
                .Include(e => e.FieldOptions)
                .OrderBy(e => e.Order)
                .ToListAsync();
        return await Where(e => e.RegStepId == regStepId && e.ForMember)
            .Include(e => e.FieldType)
            .Include(e => e.FieldOptions)
            .OrderBy(e => e.Order)
            .ToListAsync();
    }

    public Task<List<Field>> GetAllWithOptions(int regStepId)
    {
        return Where(e => e.RegStepId == regStepId)
            .Include(e => e.FieldOptions)
            .OrderBy(e => e.Order)
            .ToListAsync();
    }
}