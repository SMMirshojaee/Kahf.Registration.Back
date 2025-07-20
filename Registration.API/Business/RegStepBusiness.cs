using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Registration.API.Business;

public class RegStepBusiness(RegContext context, IMapper mapper) : GenericBusiness<RegStep>(context, mapper)
{
    public Task<List<RegStep>> GetByRegId(int regId)
        => Where(e => e.RegId == regId)
            .Include(e => e.RegStepStatuses)
            .Include(e => e.Step)
            .OrderBy(e => e.Order).ToListAsync();

    public Task<RegStep?> GetByIdWithStatuses(int id)
    =>
        Where(e => e.Id == id)
            .Include(e => e.RegStepStatuses)
            .FirstOrDefaultAsync();

    public async Task<RegStep?> GetNextStep(int currentRegStepId)
    {
        int regId = (await GetById(currentRegStepId))!.RegId;
        List<RegStep> regSteps = await GetByRegId(regId);
        if (regSteps is { Count: > 0 })
        {
            int index = regSteps.FindIndex(e => e.Id == currentRegStepId);
            if (index == -1)
                return null;
            if (index == regSteps.Count - 1)
                return null;
            return regSteps[index + 1];
        }

        return null;
    }
}