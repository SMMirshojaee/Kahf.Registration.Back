using AutoMapper;
using Registration.API.Entity.Dtos;

namespace Registration.API.Common
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Field, FieldDto>().ReverseMap();
            CreateMap<FieldType, FieldTypeDto>().ReverseMap();
            CreateMap<FieldOption, FieldOptionDto>().ReverseMap();

            CreateMap<Reg, RegDto>().ReverseMap();
            CreateMap<RegCost, RegCostDto>().ReverseMap();
            CreateMap<Step, StepDto>().ReverseMap();
            CreateMap<RegStep, RegStepDto>().ReverseMap();
            CreateMap<RegStepStatus, RegStepStatusDto>().ReverseMap();

            CreateMap<ApplicantExtraCost, ApplicantExtraCostDto>().ReverseMap();
            CreateMap<ApplicantFormValue, ApplicantFormValueDto>().ReverseMap();
            CreateMap<Applicant, MemberInfoDto>().ReverseMap();
            CreateMap<Applicant, ApplicantInfoDto>().ReverseMap();
            CreateMap<Applicant, ApplicantWithFormValueDto>().ReverseMap();
            CreateMap<Message, MessageDto>().ReverseMap();

            CreateMap<Entity.Models.Payment, PaymentDto>().ReverseMap();
            CreateMap<Order, OrderDto>().ReverseMap();
        }
    }
}
