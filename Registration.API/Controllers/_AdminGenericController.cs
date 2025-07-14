using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Registration.API.Business;
using Registration.API.Common;

namespace Registration.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize("Admin")]
    public class AdminGenericController<TBusiness, TEntity> : ControllerBase
        where TBusiness : GenericBusiness<TEntity> where TEntity : BaseEntity
    {
        protected TBusiness Business { get; init; }
        protected IMapper Mapper { get; init; }
        protected AppSettings AppSetting { get; init; }
        public AdminGenericController(TBusiness business, IMapper mapper, IOptions<AppSettings> appSetting,
            IHttpContextAccessor contextAccessor)
        {
            Business = business;
            Mapper = mapper;
            AppSetting = appSetting.Value;
        }

    }
}
