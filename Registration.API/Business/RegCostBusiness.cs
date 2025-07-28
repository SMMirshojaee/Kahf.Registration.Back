using AutoMapper;

namespace Registration.API.Business;

public class RegCostBusiness(RegContext context, IMapper mapper) : GenericBusiness<RegCost>(context, mapper)
{
}