using System.Net;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Registration.API.Common;
using Registration.API.Entity.Dtos;

namespace Registration.API.Business;

public class ApplicantBusiness(RegContext context, IMapper mapper) : GenericBusiness<Applicant>(context, mapper)
{
    public async Task<ActionReport> Signup(SignupDto signupForm, int regId)
    {
        Applicant? applicant = await FirstOrDefaultAsync(e =>
            e.RegId == regId && (e.NationalNumber == signupForm.NationalCode || e.PhoneNumber == signupForm.Mobile));
        if (applicant is not null)
            return ActionReport.Error(HttpStatusCode.Conflict, "کد ملی یا شماره موبایل تکراری است");
        var t = await Add(new Applicant
        {
            RegId = regId,
            PhoneNumber = signupForm.Mobile,
            NationalNumber = signupForm.NationalCode,
        });
        return t;
    }
}