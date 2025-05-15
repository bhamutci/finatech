namespace FinaTech.Tests.Services;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

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
        const string bic = "BIC0001";
        var cancellationToken = CancellationToken.None;

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
            Name = accountName,
            AccountNumber = "12345",
            Iban = iban,
            Bic = bic,
            Address = address,
        };

        _dbContext.Accounts.Add(accountEntity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var expectedAccountDto = new AccountDto(accountId, address.Id, accountName, accountNumber, iban, null, null);

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
        }

    }

    [Test]
    public async Task GetAccountsAsync_ShouldApplyKeywordFilter()
    {
        var totalNumberOfAccounts = 10;
        var sampleAccounts = GetSampleAccounts(totalNumberOfAccounts);

        // Modify some accounts to include a specific keyword in Name or AddressLine1
        sampleAccounts[0].Name = "Special Account NYC";
        sampleAccounts[3].Address.AddressLine1 = "123 Special Lane";
        sampleAccounts[5].Iban = "SPECIALIBAN123"; // Add filter for Iban too
        sampleAccounts[7].AccountNumber = "SPECIALACC456"; // Add filter for AccountNumber
        sampleAccounts[9].Address.City = "SpecialCity"; // Add filter for City
        sampleAccounts[1].Address.CountryCode = "SP"; // Add filter for CountryCode


        _dbContext.Accounts.AddRange(sampleAccounts);
        await _dbContext.SaveChangesAsync();

        var keyword = "Special";
        var skipCount = 0;
        var maxResultCount = 10;

        // Expected count after filtering
        var expectedFilteredEntities = sampleAccounts.Where(a =>
            (a.Name != null && a.Name.Contains(keyword)) ||
            (a.Iban != null && a.Iban.Contains(keyword)) || // Include Iban filter
            (a.AccountNumber != null && a.AccountNumber.Contains(keyword)) || // Include AccountNumber filter
            (a.Address != null && a.Address.AddressLine1 != null &&
             a.Address.AddressLine1.Contains(keyword)) || // Include Address filters
            (a.Address != null && a.Address.AddressLine2 != null && a.Address.AddressLine2.Contains(keyword)) ||
            (a.Address != null && a.Address.AddressLine3 != null && a.Address.AddressLine3.Contains(keyword)) ||
            (a.Address != null && a.Address.City != null && a.Address.City.Contains(keyword)) ||
            (a.Address != null && a.Address.PostCode != null && a.Address.PostCode.Contains(keyword)) ||
            (a.Address != null && a.Address.CountryCode != null && a.Address.CountryCode.Contains(keyword))
        ).ToList();
        var expectedFilteredCount = expectedFilteredEntities.Count;


        // Create the filter with the keyword
        var accountFilter = new AccountFilter
        {
            Keywords = keyword,
            SkipCount = skipCount,
            MaxResultCount = maxResultCount,
        };

        CancellationToken cancellationToken = CancellationToken.None;
        var result = await _accountService.GetAccountsAsync(accountFilter, cancellationToken);

        // Assert: Verify the outcome
        ClassicAssert.NotNull(result);
        ClassicAssert.NotNull(result.Items);

        // Total count should match the number of items that match the filter *before* pagination
        ClassicAssert.AreEqual(expectedFilteredCount, result.TotalCount);
        // Item count should match the number of items in the page (up to MaxResultCount)
        ClassicAssert.AreEqual(expectedFilteredCount, result.Items.Count); // Assuming MaxResultCount is large enough

        // Verify the IDs of the returned items match the expected filtered entities
        var expectedFilteredIds = expectedFilteredEntities.Select(a => a.Id).ToList();
        var actualFilteredIds = result.Items.Select(dto => dto.Id).ToList();
        CollectionAssert.AreEquivalent(expectedFilteredIds, actualFilteredIds);
    }

    [Test]
    public async Task GetAccountsAsync_ShouldReturnEmptyList_WhenNoAccountsMatchFilter()
    {
        var totalNumberOfAccounts = 10;
        var sampleAccounts = GetSampleAccounts(totalNumberOfAccounts);
        _dbContext.Accounts.AddRange(sampleAccounts);
        await _dbContext.SaveChangesAsync();

        // Create a filter that won't match any data
        var accountFilter = new AccountFilter
        {
            Keywords = "NonExistentKeyword", // Use a keyword that doesn't exist
            SkipCount = 0,
            MaxResultCount = 10,
        };

        CancellationToken cancellationToken = CancellationToken.None;
        var result = await _accountService.GetAccountsAsync(accountFilter, cancellationToken);

        // Assert: Verify the outcome
        ClassicAssert.NotNull(result);
        ClassicAssert.NotNull(result.Items);

        // Total count should be 0
        ClassicAssert.AreEqual(0, result.TotalCount);
        // Item count should be 0
        ClassicAssert.AreEqual(0, result.Items.Count);
    }

    [Test]
    public async Task CreateAccountAsync_ShouldCreateAccount_WhenValidAccountDto()
    {
        var validAccountDto = new CreateAccountDto(
            Name: "New Test Account",
            AccountNumber: "98765",
            Iban: "NL99FINA9876543210",
            Bic: "BIC0001",
            Address: new AddressDto(0, "New Address Line 1", null, null, "Amsterdam", "1000AA",
                "NL")
        );
        var cancellationToken = CancellationToken.None;

        var createdAccountDto = await _accountService.CreateAccountAsync(validAccountDto, cancellationToken);

        ClassicAssert.NotNull(createdAccountDto);

        ClassicAssert.That(createdAccountDto.Id, Is.GreaterThan(0));

        ClassicAssert.AreEqual(validAccountDto.Name, createdAccountDto.Name);
        ClassicAssert.AreEqual(validAccountDto.Iban, createdAccountDto.Iban);

        var accountInDb = await _dbContext.Accounts.FindAsync([createdAccountDto.Id], cancellationToken);
        ClassicAssert.NotNull(accountInDb);
        ClassicAssert.AreEqual(validAccountDto.Name, accountInDb.Name);
    }


    [Test]
    public async Task CreateAccountAsync_ShouldThrowArgumentException_WhenAccountDtoIsInvalid()
    {
        // Arrange: Create an invalid AccountDto (e.g., missing required Name)
        var invalidAccountDto = new CreateAccountDto(
            Name: null,
            AccountNumber: "98765",
            Iban: "NL99FINA9876543210",
            Bic: "BIC0001",
            Address: new AddressDto(0, "New Address Line 1", null, null, "Amsterdam", "1000AA", "NL")
        );

        var cancellationToken = CancellationToken.None;

        var thrownException = ClassicAssert.ThrowsAsync<ArgumentNullException>(async () =>
            await _accountService.CreateAccountAsync(invalidAccountDto, cancellationToken)
        );

        ClassicAssert.That(thrownException.Message, Contains.Substring("Account name cannot be null or empty"));

        var accountCountAfter = await _dbContext.Accounts.CountAsync(cancellationToken: cancellationToken);
        ClassicAssert.AreEqual(0, accountCountAfter);
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }

    private List<Core.Account> GetSampleAccounts(int count)
    {
        var accounts = new List<Core.Account>();


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
                Bic = "BIC0001",
                AddressId = address.Id,
                Address = address,
            });
        }

        return accounts;
    }

    private List<AccountDto> GetSampleAccountDtos(List<Core.Account> entities)
    {
        var address = new AddressDto(1, "Test St", "", "", "", "", "GB");

        return entities.Select(entity => new AccountDto(entity.Id, address.Id, entity.Name, "12345", "GB12FINA1234567890","BIC0001", address)).ToList();
    }

}
