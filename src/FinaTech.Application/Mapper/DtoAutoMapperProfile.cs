using FinaTech.Application.Services.Bank.Dto;

namespace FinaTech.Application.Mapper;

using AutoMapper;
using Core;
using Services.Payment.Dto;

public class DtoAutoMapperProfile: Profile
{
    public DtoAutoMapperProfile()
    {
        PaymentProfile();
        BankProfile();
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

    private void BankProfile()
    {
        CreateMap<Bank, BankDto>()
            .AfterMap(MapAccountDto)
            .ReverseMap();
    }

    private void MapAccountDto(Bank bank, BankDto bankDto)
    {
        foreach (var account in bankDto.Accounts)
        {
            bankDto.Accounts.Add(account);
        }
    }



}
