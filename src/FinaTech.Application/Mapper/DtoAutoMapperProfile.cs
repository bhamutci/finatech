namespace FinaTech.Application.Mapper;

using AutoMapper;

using Core;
using Services.Payment.Dto;
using Services.Account.Dto;
using Services.Bank.Dto;

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
        BankProfile();
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
        CreateMap<Address, AddressDto>()
            .MaxDepth(1)
            .ReverseMap()
            .MaxDepth(1);
    }

    /// <summary>
    /// Configures the mapping rules for the Account entity and its corresponding data transfer object (AccountDto)
    /// using the AutoMapper library. This profile ensures that properties between Account and AccountDto are properly mapped,
    /// including support for reverse mapping.
    /// </summary>
    private void AccountProfile()
    {
        CreateMap<Account, AccountDto>()
            .MaxDepth(1)
            .ReverseMap()
            .MaxDepth(1);

        CreateMap<CreateAccountDto, Account>()
            .MaxDepth(1);
    }

    /// <summary>
    /// Configures the mapping rules for the Money entity and its corresponding data transfer object (MoneyDto)
    /// using the AutoMapper library. This profile ensures that properties between Money and MoneyDto are properly mapped,
    /// including support for reverse mapping.
    /// </summary>
    private void MoneyProfile()
    {
        CreateMap<Money, MoneyDto>()
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
        CreateMap<Payment, PaymentDto>()
            .MaxDepth(1)
            .ForMember(member => member.ChargesBearer,
                opt => opt.MapFrom(p => (ChargesBearer) p.ChargesBearer))
            .ReverseMap()
            .MaxDepth(1)
            .ForMember(member =>member.ChargesBearer,
                opt=>opt.MapFrom(p=>(int)p.ChargesBearer));
    }

    /// <summary>
    /// Configures the mapping rules for the Bank entity and its corresponding data transfer object (BankDto)
    /// using the AutoMapper library. This profile ensures proper property mappings between Bank and BankDto,
    /// including handling of associated Account objects.
    /// </summary>
    private void BankProfile()
    {
        CreateMap<Bank, BankDto>()
            .MaxDepth(1)
            .AfterMap(MapAccountDto)
            .ReverseMap()
            .MaxDepth(1);
    }

    /// <summary>
    /// Maps account data from the Bank domain model to the BankDto data transfer object.
    /// Specifically, it ensures that accounts from the Bank object are correctly added
    /// to the corresponding Accounts collection in the BankDto.
    /// </summary>
    /// <param name="bank">The source domain model representing a bank and its associated data.</param>
    /// <param name="bankDto">The destination data transfer object to which account data is mapped.</param>
    private void MapAccountDto(Bank bank, BankDto bankDto)
    {
        /*foreach (var account in bankDto.Accounts)
        {
            bankDto.Accounts.Add(account);
        }*/
    }
}
