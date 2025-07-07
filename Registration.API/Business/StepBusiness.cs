using AutoMapper;

namespace Registration.API.Business;

public class StepBusiness(RegContext context, IMapper mapper) : GenericBusiness<Step>(context, mapper)
{
}