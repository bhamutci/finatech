namespace FinaTech.Application.Mapper;

using AutoMapper;
using Services.Payment.Dto;
public class DtoAutoMapperProfile: Profile
{
    /// <summary>
    /// A class that defines mapping configurations for the AutoMapper library.
    /// This profile includes object mappings for various domain entities and their corresponding
    /// data transfer objects (DTOs) such as Address, Account, Payment, and Bank models.
    /// </summary>
    public DtoAutoMapperProfile()
    {
        AddressProfile();
        AccountProfile();
        MoneyProfile();
        PaymentProfile();
    }

    /// <summary>
    /// Configures the mapping rules for the Address entity and its corresponding data transfer object (AddressDto)
    /// using the AutoMapper library. This profile ensures that properties between Address and AddressDto are properly mapped,
    /// including support for reverse mapping.
    /// </summary>
    private void AddressProfile()
    {
        CreateMap<Core.Account.Address, Address>()
            .MaxDepth(1);
        CreateMap<CreateAddress, Core.Account.Address>();
    }

    /// <summary>
    /// Configures the mapping rules for the Account entity and its corresponding data transfer object (AccountDto)
    /// using the AutoMapper library. This profile ensures that properties between Account and AccountDto are properly mapped,
    /// including support for reverse mapping.
    /// </summary>
    private void AccountProfile()
    {
        CreateMap<Core.Account.Account, Account>()
            .MaxDepth(1)
            .ReverseMap()
            .MaxDepth(1);

        CreateMap<CreateAccount, Core.Account.Account>()
            .MaxDepth(1);
    }

    /// <summary>
    /// Configures the mapping rules for the Money entity and its corresponding data transfer object (MoneyDto)
    /// using the AutoMapper library. This profile ensures that properties between Money and MoneyDto are properly mapped,
    /// including support for reverse mapping.
    /// </summary>
    private void MoneyProfile()
    {
        CreateMap<Core.Payment.Money, Money>()
            .MaxDepth(1)
            .ReverseMap()
            .MaxDepth(1);
    }

    /// <summary>
    /// Configures mapping for payment entities and their Data Transfer Objects (DTOs).
    /// Includes transformation logic for the ChargesBearer property between domain models and DTOs.
    /// </summary>
    private void PaymentProfile()
    {
        CreateMap<Core.Payment.Payment, Payment>()
            .MaxDepth(1)
            .ForMember(member => member.ChargesBearer,
                opt => opt.MapFrom(p => (ChargesBearer) p.ChargesBearer))
            .ReverseMap()
            .MaxDepth(1)
            .ForMember(member =>member.ChargesBearer,
                opt=>opt.MapFrom(p=>(int)p.ChargesBearer));

        CreateMap<CreatePayment, Core.Payment.Payment>();
    }
}
