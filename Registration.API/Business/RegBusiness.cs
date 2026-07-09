using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Registration.API.Business;

public class RegBusiness(RegContext context, IMapper mapper) : GenericBusiness<Reg>(context, mapper)
{
    public Task<List<Reg>> GetActiveRegs()
     => Where(e => e.IsActive).ToListAsync();
}