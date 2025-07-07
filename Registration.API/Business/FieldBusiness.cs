using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Registration.API.Entity.Dtos;

namespace Registration.API.Business
{
    public class FieldBusiness(RegContext context, IMapper mapper) : GenericBusiness<Field>(context, mapper)
    {
        public Task<List<Field>> GetByRegStepId(int regStepId) =>
            Where(e => e.RegStepId == regStepId)
                .Include(e => e.FieldType)
                .Include(e => e.FieldOptions)
                .ToListAsync();
    }
}
