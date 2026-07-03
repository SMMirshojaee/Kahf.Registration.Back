using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Registration.API.Business;

public class RegStepStatusBusiness(RegContext context, IMapper mapper) : GenericBusiness<RegStepStatus>(context, mapper)
{
    public async Task<List<RegStepStatus>?> GetSameStepStatuses(int statusId)
    {
        RegStepStatus? status = await FirstOrDefaultAsync(e => e.Id == statusId);
        if (status is null)
            return null;

        return await Where(e => e.RegStepId == status.RegStepId).ToListAsync();
    }
}