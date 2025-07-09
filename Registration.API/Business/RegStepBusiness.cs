using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Registration.API.Business;

public class RegStepBusiness(RegContext context, IMapper mapper) : GenericBusiness<RegStep>(context, mapper)
{
    public Task<List<RegStep>> GetByRegId(int regId)
        => Where(e => e.RegId == regId).OrderBy(e => e.Order).ToListAsync();
}