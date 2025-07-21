using AutoMapper;

namespace Registration.API.Business
{
    public class MessageBusiness(RegContext context, IMapper mapper) : GenericBusiness<Message>(context, mapper)
    {



    }
}
