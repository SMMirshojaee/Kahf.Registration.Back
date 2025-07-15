using AutoMapper;
using Microsoft.Extensions.Options;
using Registration.API.Business;
using Registration.API.Common;

namespace Registration.API.Controllers.Admin
{
    public class AdminController(ApplicantBusiness b, IMapper m, IOptions<AppSettings> ap, IHttpContextAccessor ac) :
        AdminGenericController<ApplicantBusiness, Applicant>(b, m, ap, ac)
    {
    }
}
