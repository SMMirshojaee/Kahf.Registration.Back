using AutoMapper;

namespace Registration.API.Business;

public class RegBusiness(RegContext context, IMapper mapper) : GenericBusiness<Reg>(context, mapper)
{
}