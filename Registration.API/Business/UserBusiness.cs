using AutoMapper;

namespace Registration.API.Business;

public class UserBusiness(RegContext c, IMapper m) : GenericBusiness<User>(c, m)
{
    public Task<User?> GetByUsername(string username)
        => FirstOrDefaultAsync(e => e.Username == username);
}