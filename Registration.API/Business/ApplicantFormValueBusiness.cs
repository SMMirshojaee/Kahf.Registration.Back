using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Registration.API.Business
{
    public class ApplicantFormValueBusiness(RegContext context, IMapper mapper) : GenericBusiness<ApplicantFormValue>(context, mapper)
    {
        public Task<List<ApplicantFormValue>> GetByApplicantIdAndRegStepId(int applicantId, int regStepId)
        => Where(e => e.ApplicantId == applicantId && e.RegStepId == regStepId)
                .Include(e => e.Field)
                .Include(e => e.FieldOption)
                .ToListAsync();
    }
}
