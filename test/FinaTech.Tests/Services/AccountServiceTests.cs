using FinaTech.Application.Services.Bank.Dto;

namespace FinaTech.Tests.Services;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

using Moq;
using AutoMapper;
using NUnit.Framework;
using NUnit.Framework.Legacy;

using Application.Mapper;
using EntityFramework.PostgresSqlServer;
using FinaTech.Application.Services.Account;
using FinaTech.Application.Services.Account.Dto;



[TestFixture]
public class AccountServiceTests
{
    private FinaTechPostgresSqlDbContext _dbContext;
    private IMapper _mapper;
    private ILogger<AccountService> _mockLogger;
    private AccountService _accountService;


    [SetUp]
    public void Setup()
    {
        var serviceProvider = new ServiceCollection()
            .AddLogging()
            .AddAutoMapper(typeof(DtoAutoMapperProfile).Assembly)
            .AddDbContext<FinaTechPostgresSqlDbContext>(options => options.UseInMemoryDatabase("ServicesDB"))
            .BuildServiceProvider();

        var factory = serviceProvider.GetService<ILoggerFactory>();

        _mockLogger = factory.CreateLogger<AccountService>();
        _mapper = serviceProvider.GetService<IMapper>();
        _dbContext = serviceProvider.GetService<FinaTechPostgresSqlDbContext>();
        _accountService = new AccountService(_dbContext, _mapper, _mockLogger);
    }

    [Test]
    public async Task GetAccountAsync_ShouldReturnNull_WhenAccountNotFound()
    {
        var nonExistentAccountId = 999;
        var cancellationToken = CancellationToken.None;

        var result = await _accountService.GetAccountAsync(nonExistentAccountId, cancellationToken);

        ClassicAssert.Null(result);
    }

    [Test]
    public async Task GetAccountAsync_ShouldReturnAccountDto_WhenAccountFound()
    {
        const int accountId = 123;
        const string accountName = "Test Account Name";
        const string iban = "GB12FINA1234567890";
        const string accountNumber = "12345";
        var cancellationToken = CancellationToken.None;

        // Add the Bank entity to the in-memory database
        var bank = new Core.Bank()
        {
            Id = 1,
            Name = "Test Bank",
            BIC = "BIC0001"
        };
        _dbContext.Banks.Add(bank);

        // Add the Address entity to the in-memory database
        var address = new Core.Address()
        {
            Id = 1,
            CountryCode = "GB",
            AddressLine1 = "Test St"
        };
        _dbContext.Addresses.Add(address);

        // Add the Account entity to the in-memory database
        var accountEntity = new Core.Account
        {
            Id = accountId,
            AddressId = address.Id,
            BankId = bank.Id,
            Name = accountName,
            AccountNumber = "12345",
            Iban = iban,
            Address = address,
            Bank = bank
        };

        _dbContext.Accounts.Add(accountEntity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var expectedAccountDto = new AccountDto(accountId, bank.Id, address.Id, accountName, accountNumber, iban, null, null);

        // Act: Call the method being tested
        var resultDto = await _accountService.GetAccountAsync(accountId, cancellationToken);

        // Assert: Verify the outcome
        ClassicAssert.NotNull(resultDto);
        ClassicAssert.AreEqual(accountId, resultDto.Id);
        ClassicAssert.AreEqual(expectedAccountDto.Name, resultDto.Name);
    }

    [Test]
    public async Task GetAccountsAsync_ShouldReturnPaginatedResult_WhenNoFilterApplied()
    {
        var totalNumberOfAccounts = 20;
        var skipCount = 5;
        var maxResultCount = 10;
        var cancellationToken = CancellationToken.None;

        var sampleAccounts = GetSampleAccounts(totalNumberOfAccounts);
        _dbContext.Accounts.AddRange(sampleAccounts);

        await _dbContext.SaveChangesAsync(cancellationToken);

        var expectedEntitiesInPage = sampleAccounts.Skip(skipCount).Take(maxResultCount).ToList();
        var expectedDtosInPage = GetSampleAccountDtos(expectedEntitiesInPage);

        var accountFilter = new AccountFilter
        {
            SkipCount = skipCount,
            MaxResultCount = maxResultCount,
            Keywords = null,
        };

        var result = await _accountService.GetAccountsAsync(accountFilter, cancellationToken);

        // Assert: Verify the outcome
        ClassicAssert.NotNull(result);
        ClassicAssert.NotNull(result.Items);

        // Assert the total count matches the total number of items added
        ClassicAssert.AreEqual(totalNumberOfAccounts, result.TotalCount);

        // Assert the number of items in the returned page matches the take count
        ClassicAssert.AreEqual(maxResultCount, result.Items.Count);

        // Assert that the items in the result match the expected DTOs for the page
        // You could compare them by ID or other key properties
        for (int i = 0; i < maxResultCount; i++)
        {
            ClassicAssert.AreEqual(expectedDtosInPage[i].Id, result.Items[i].Id);
            // Add more assertions to compare other properties if needed
            ClassicAssert.AreEqual(expectedDtosInPage[i].Name, result.Items[i].Name);
            // etc.
        }

    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.Dispose();
    }

    // Helper to create sample Account entities
    private List<Core.Account> GetSampleAccounts(int count)
    {
        var accounts = new List<Core.Account>();

        var bank = new Core.Bank
        {
            Id = 1,
            Name = "Test Bank",
            BIC = "BIC0001"
        };

        var address = new Core.Address
        {
            Id = 1,
            CountryCode = "GB",
            AddressLine1 = "Test St"
        };

        for (int i = 1; i <= count; i++)
        {

            accounts.Add(new Core.Account
            {
                Id = i,
                Name = $"Account {i}",
                AccountNumber = $"ACC{i.ToString().PadLeft(4, '0')}",
                Iban = $"IBAN{i.ToString().PadLeft(10, '0')}",
                BankId = bank.Id,
                AddressId = address.Id,
                Address = address,
                Bank = bank
            });
        }

        return accounts;
    }

    private List<AccountDto> GetSampleAccountDtos(List<Core.Account> entities)
    {
        var bank = new BankDto(1, "Test Bank", "BIC0001");

        var address = new AddressDto(1, "Test St", "", "", "", "", "GB");

        return entities.Select(entity => new AccountDto(entity.Id, bank.Id, address.Id, entity.Name, "12345", "GB12FINA1234567890", address, bank)).ToList();
    }

}
