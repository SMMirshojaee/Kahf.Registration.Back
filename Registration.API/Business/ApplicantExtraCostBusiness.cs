using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Registration.API.Entity.Dtos;

namespace Registration.API.Business;

public class ApplicantExtraCostBusiness(RegContext context, IMapper mapper) : GenericBusiness<ApplicantExtraCost>(context, mapper)
{
    public Task<List<ApplicantExtraCost>> GetByApplicantId(int applicantId)
        => Where(e => e.ApplicantId == applicantId).ToListAsync();
}