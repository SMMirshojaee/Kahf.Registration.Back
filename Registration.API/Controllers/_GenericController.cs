using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Registration.API.Business;
using Registration.API.Common;

namespace Registration.API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class GenericController<TBusiness, TEntity>(TBusiness business, IMapper mapper, IOptions<AppSettings> appSetting) : ControllerBase
        where TBusiness : GenericBusiness<TEntity> where TEntity : BaseEntity
    {
        protected TBusiness Business => business;
        protected IMapper Mapper => mapper;
        protected AppSettings AppSetting => appSetting.Value;

        protected IActionResult Status(ActionReport report)
        {
            if (report.Successful)
                return Ok(report.Message);
            if (report.Code == HttpStatusCode.Conflict)
                return StatusCode((int)HttpStatusCode.Conflict, report.Message);
            if (report.Code == HttpStatusCode.NotFound)
                return StatusCode((int)HttpStatusCode.NotFound, report.Message);
            return StatusCode((int)HttpStatusCode.InternalServerError, report.Message);
        }
    }
}
