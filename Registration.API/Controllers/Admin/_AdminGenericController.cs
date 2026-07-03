using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Registration.API.Business;
using Registration.API.Common;
using Registration.API.Entity.Models;
using System.Security.Claims;

namespace Registration.API.Controllers.Admin
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
        protected readonly int UserId;
        public AdminGenericController(TBusiness business, IMapper mapper, IOptions<AppSettings> appSetting,
            IHttpContextAccessor contextAccessor)
        {
            Business = business;
            Mapper = mapper;
            AppSetting = appSetting.Value;

            ClaimsPrincipal? user = contextAccessor.HttpContext?.User;
            if (user?.Identity is null || !user.Identity.IsAuthenticated)
                return;
            try
            {

                string? actor = user.FindFirst(ClaimTypes.Actor)?.Value;
                switch (actor)
                {
                    case "Admin":
                        UserId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                        return;
                    case "SuperAdmin":
                        UserId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                        return;
                    default:
                        return;
                }
            }
            catch (Exception e)
            {
            }
        }
        protected IActionResult Status(ActionReport report)
        {
            if (report.Successful)
                return Ok();
            return StatusCode((int)report.Code, report.Message);
        }

    }
}
