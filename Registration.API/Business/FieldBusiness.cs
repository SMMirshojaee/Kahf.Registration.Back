using Microsoft.EntityFrameworkCore;

namespace Registration.API.Business
{
    public class FieldBusiness(RegContext context) : GenericBusiness<Fields>(context)
    {
        public Task<List<Fields>> GetByRegStepId(int regStepId) =>
            Where(e => e.RegStepId == regStepId)
                .Include(e => e.FieldType)
                //.Include(e=>e.)
                .ToListAsync();

    }
}
