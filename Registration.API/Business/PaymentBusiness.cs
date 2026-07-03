using AutoMapper;

namespace Registration.API.Business;

using Payment = Entity.Models.Payment;

public class PaymentBusiness(RegContext context, IMapper mapper) : GenericBusiness<Payment>(context, mapper)
{

    public Task<Payment?> GetByRegStepId(int regStepId) =>
        FirstOrDefaultAsync(e => e.RegStepId == regStepId);
}