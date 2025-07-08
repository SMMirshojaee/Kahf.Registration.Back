using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Registration.API.Business;
using Registration.API.Common;

namespace Registration.API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class GenericController<TBusiness, TEntity> : ControllerBase
        where TBusiness : GenericBusiness<TEntity> where TEntity : BaseEntity
    {
        protected TBusiness Business { get; init; }
        protected IMapper Mapper { get; init; }
        protected AppSettings AppSetting { get; init; }

        private readonly bool _userIsAuthenticated = false;

        protected readonly int ApplicantId;
        protected readonly int RegId;
        protected readonly string NationalCode;
        protected readonly string Mobile;

        public GenericController(TBusiness business, IMapper mapper, IOptions<AppSettings> appSetting, IHttpContextAccessor contextAccessor)
        {
            Business = business;
            Mapper = mapper;
            AppSetting = appSetting.Value;
            ClaimsPrincipal? user = contextAccessor.HttpContext?.User;
            _userIsAuthenticated = false;
            if (user?.Identity is null || !user.Identity.IsAuthenticated)
                return;
            try
            {

                string? actor = user.FindFirst(ClaimTypes.Actor)?.Value;
                switch (actor)
                {
                    case "Applicant":
                        ApplicantId = int.Parse(user.FindFirst(ClaimTypes.SerialNumber)?.Value!);
                        RegId = int.Parse(user.FindFirst(ClaimTypes.PrimarySid)?.Value!);
                        NationalCode = user.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
                        Mobile = user.FindFirst(ClaimTypes.MobilePhone)?.Value!;
                        return;
                    case "Admin":
                        return;
                    case "SuperAdmin":

                        return;
                    default:
                        return;
                }

            }
            catch (Exception e)
            {
                _userIsAuthenticated = false;
            }
        }

        protected IActionResult Status<T>(ActionReport<T> report)
        {
            if (report.Successful)
                return Ok(report.Output);
            if (report.Code == HttpStatusCode.Conflict)
                return StatusCode((int)HttpStatusCode.Conflict, report.Message);
            if (report.Code == HttpStatusCode.NotFound)
                return StatusCode((int)HttpStatusCode.NotFound, report.Message);
            return StatusCode((int)HttpStatusCode.InternalServerError, report.Message);
        }
    }
}
