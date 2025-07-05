using Microsoft.EntityFrameworkCore;

namespace Registration.API.Business
{
    public class FieldBusiness(RegContext context) : GenericBusiness<Field>(context)
    {
        public Task<List<Field>> GetByRegStepId(int regStepId) =>
            Where(e => e.RegStepId == regStepId)
                .Include(e => e.FieldType)
                //.Include(e => e.FieldOptions)
                .ToListAsync();
    }
}
