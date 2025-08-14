using AutoMapper;

namespace Registration.API.Business
{
    public class ApplicantExtraCostBusiness(RegContext context, IMapper mapper) : GenericBusiness<ApplicantExtraCost>(context, mapper)
    {
    }
}
