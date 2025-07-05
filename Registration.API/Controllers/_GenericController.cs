using Microsoft.AspNetCore.Mvc;
using Registration.API.Business;

namespace Registration.API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class GenericController<TBusiness, TEntity>(TBusiness business) : ControllerBase
        where TBusiness : GenericBusiness<TEntity> where TEntity : BaseEntity
    {
        protected TBusiness Business => business;
    }
}
