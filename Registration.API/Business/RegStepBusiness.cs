using AutoMapper;

namespace Registration.API.Business;

public class RegStepBusiness(RegContext context, IMapper mapper) : GenericBusiness<Step>(context, mapper)
{
}