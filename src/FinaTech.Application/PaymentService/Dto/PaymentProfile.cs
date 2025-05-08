using AutoMapper;
using FinaTech.Core;

namespace FinaTech.Application.PaymentService.Dto;

public class PaymentProfile: Profile
{
    public PaymentProfile()
    {
        CreateMap<Payment, PaymentDto>()
            .ForMember(member => member.ChargesBearer,
                opt => opt.MapFrom(p => (ChargesBearer) p.ChargesBearer))
            .ReverseMap()
            .ForMember(member =>member.ChargesBearer,
                opt=>opt.MapFrom(p=>(int)p.ChargesBearer));

    }
}
