namespace FinaTech.Application.Mapper;

using AutoMapper;
using Core;
using Services.Payment.Dto;

public class DtoAutoMapperProfile: Profile
{
    public DtoAutoMapperProfile()
    {
        PaymentProfile();
    }

    private void PaymentProfile()
    {
        CreateMap<Payment, PaymentDto>()
            .ForMember(member => member.ChargesBearer,
                opt => opt.MapFrom(p => (ChargesBearer) p.ChargesBearer))
            .ReverseMap()
            .ForMember(member =>member.ChargesBearer,
                opt=>opt.MapFrom(p=>(int)p.ChargesBearer));
    }

}
