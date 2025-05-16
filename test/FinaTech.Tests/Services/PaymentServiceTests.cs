namespace FinaTech.Tests.Services;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

using AutoMapper;
using FluentValidation;
using NUnit.Framework;
using NUnit.Framework.Legacy;


using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using Core;
using Application.Mapper;
using EntityFramework.PostgresSqlServer;
using FinaTech.Application.Services.Payment.Dto;
using FinaTech.Application.Services.Account.Dto;
using FinaTech.Application.Services.Payment;

[TestFixture]
public class PaymentServiceTests
{
    private FinaTechPostgresSqlDbContext _dbContext;
    private IMapper _mapper;
    private ILogger<PaymentService> _logger;
    private PaymentService _paymentService;
    private IValidator<CreatePaymentDto?> _validator;
    private static readonly Address Address = new() {Id = 1, CountryCode = "GB", AddressLine1 = "Test St"};
    private static readonly AddressDto AddressDto = new(1,"Test St",string.Empty, string.Empty, string.Empty, string.Empty, CountryCode:"GB");

    private List<Payment> GetSamplePayments(int count, int startId = 1)
    {
        var payments = new List<Payment>();

        Account beneficiaryAccount = new Account
        {
            Id = 123,
            AddressId = Address.Id,
            Name = "BeneficiaryAccount",
            AccountNumber = "12344",
            Iban = "GB12FINA1234567890",
            Bic="BIC0001",
            Address = Address,
        };

        Account originatorAccount = new Account
        {
            Id = 124,
            AddressId = Address.Id,
            Name = "OriginatorAccount",
            AccountNumber = "12345",
            Iban = "GB12FINA1234567891",
            Bic = "BIC0001",
            Address = Address,
        };

        for (int i = 0; i < count; i++)
        {
            var id = startId + i;
            payments.Add(new Payment
            {
                Id = id,
                ReferenceNumber = $"REF{id.ToString().PadLeft(5, '0')}",
                Details = $"Details for payment {id}",
                Amount = new Money(10,
                    "GB"),
                BeneficiaryAccountId = beneficiaryAccount.Id,
                OriginatorAccountId = originatorAccount.Id,
                Date = DateTimeOffset.Now,
                ChargesBearer = (int)ChargesBearer.Originator,
                OriginatorAccount = originatorAccount,
                BeneficiaryAccount = beneficiaryAccount
            });
        }

        return payments;
    }

    [SetUp]
    public void Setup()
    {
        var serviceProvider = new ServiceCollection()
            .AddLogging()
            .AddAutoMapper(typeof(DtoAutoMapperProfile).Assembly)
            .AddDbContext<FinaTechPostgresSqlDbContext>(options => options.UseInMemoryDatabase("ServicesDB"))
            .BuildServiceProvider();

        var factory = serviceProvider.GetService<ILoggerFactory>();

        _logger = factory.CreateLogger<PaymentService>();
        _mapper = serviceProvider.GetService<IMapper>();
        _dbContext = serviceProvider.GetService<FinaTechPostgresSqlDbContext>();
        _validator = serviceProvider.GetService<IValidator<CreatePaymentDto>>();

        _paymentService = new PaymentService(_dbContext, _mapper, _logger, _validator);
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext?.Dispose();
    }

    [Test]
    public async Task GetPaymentAsync_ShouldReturnPaymentDto_WhenPaymentFound()
    {
        var paymentId = 50;
        var samplePayment = GetSamplePayments(1, paymentId).Single(); // Get a single payment with specific ID

        _dbContext.Payments.Add(samplePayment);
        await _dbContext.SaveChangesAsync();

        var cancellationToken = CancellationToken.None;

        var resultDto = await _paymentService.GetPaymentAsync(paymentId, cancellationToken);

        ClassicAssert.NotNull(resultDto);
        ClassicAssert.AreEqual(paymentId, resultDto.Id);
        ClassicAssert.AreEqual(samplePayment.ReferenceNumber, resultDto.ReferenceNumber);
        ClassicAssert.AreEqual(samplePayment.Amount.Value, resultDto.Amount.Value);
        ClassicAssert.AreEqual(samplePayment.Amount.Currency, resultDto.Amount.Currency);
    }

    [Test]
    public async Task GetPaymentAsync_ShouldReturnNull_WhenPaymentNotFound()
    {
        var nonExistentPaymentId = 999;
        var cancellationToken = CancellationToken.None;

        var result = await _paymentService.GetPaymentAsync(nonExistentPaymentId, cancellationToken);

        ClassicAssert.Null(result);
    }

    [Test]
    public async Task GetPaymentsAsync_ShouldReturnPaginatedResult_WhenNoFilterApplied()
    {
        var totalNumberOfPayments = 30;
        var skipCount = 10;
        var maxResultCount = 5;
        var samplePayments = GetSamplePayments(totalNumberOfPayments);
        _dbContext.Payments.AddRange(samplePayments);
        await _dbContext.SaveChangesAsync();

        var paymentFilter = new PaymentFilter
        {
            SkipCount = skipCount,
            MaxResultCount = maxResultCount,
            Keywords = null,
            BeneficiaryAccountId = null,
            OriginatorAccountId = null
        };
        var cancellationToken = CancellationToken.None;

        var result = await _paymentService.GetPaymentsAsync(paymentFilter, cancellationToken);

        ClassicAssert.NotNull(result);
        ClassicAssert.NotNull(result.Items);

        ClassicAssert.AreEqual(totalNumberOfPayments, result.TotalCount);
        ClassicAssert.AreEqual(maxResultCount, result.Items.Count);

        var expectedIdsInPage = samplePayments.Skip(skipCount).Take(maxResultCount).Select(p => p.Id).ToList();
        var actualIdsInPage = result.Items.Select(dto => dto.Id).ToList();

        CollectionAssert.AreEquivalent(expectedIdsInPage, actualIdsInPage);
    }

    [Test]
    public async Task GetPaymentsAsync_ShouldApplyKeywordFilter()
    {
        var totalNumberOfPayments = 15;
        var samplePayments = GetSamplePayments(totalNumberOfPayments);

        samplePayments[0].ReferenceNumber = "KEYWORDREF1";
        samplePayments[5].Details = "Payment details with KEYWORD";
        samplePayments[10].ReferenceNumber = "REFKEYWORD2";

        _dbContext.Payments.AddRange(samplePayments);
        await _dbContext.SaveChangesAsync();

        var keyword = "KEYWORD";
        var skipCount = 0;
        var maxResultCount = 15;

        var expectedFilteredPayments = samplePayments.Where(p =>
            (p.ReferenceNumber != null && p.ReferenceNumber.Contains(keyword)) ||
            (p.Details != null && p.Details.Contains(keyword))
        ).ToList();
        var expectedFilteredCount = expectedFilteredPayments.Count;

        var paymentFilter = new PaymentFilter
        {
            Keywords = keyword,
            SkipCount = skipCount,
            MaxResultCount = maxResultCount,
        };
        var cancellationToken = CancellationToken.None;

        var result = await _paymentService.GetPaymentsAsync(paymentFilter, cancellationToken);

        ClassicAssert.NotNull(result);
        ClassicAssert.NotNull(result.Items);

        ClassicAssert.AreEqual(expectedFilteredCount, result.TotalCount);
        ClassicAssert.AreEqual(expectedFilteredCount, result.Items.Count);

        var expectedFilteredIds = expectedFilteredPayments.Select(p => p.Id).ToList();
        var actualFilteredIds = result.Items.Select(dto => dto.Id).ToList();
        CollectionAssert.AreEquivalent(expectedFilteredIds, actualFilteredIds);
    }

    [Test]
    public async Task GetPaymentsAsync_ShouldReturnEmptyList_WhenNoPaymentsMatchFilter()
    {
        var totalNumberOfPayments = 10;
        var samplePayments = GetSamplePayments(totalNumberOfPayments);
        _dbContext.Payments.AddRange(samplePayments);
        await _dbContext.SaveChangesAsync();

        var paymentFilter = new PaymentFilter
        {
            Keywords = "NonExistentKeyword",
            SkipCount = 0,
            MaxResultCount = 10,
        };
        var cancellationToken = CancellationToken.None;

        var result = await _paymentService.GetPaymentsAsync(paymentFilter, cancellationToken);

        ClassicAssert.NotNull(result);
        ClassicAssert.NotNull(result.Items);

        ClassicAssert.AreEqual(0, result.TotalCount);
        ClassicAssert.AreEqual(0, result.Items.Count);

    }

    [Test]
    public async Task CreatePaymentAsync_ShouldCreatePayment_WhenValidInput()
    {
        CreateAccountDto beneficiaryAccount = new CreateAccountDto("BeneficiaryAccount", "GB12FINA1234567891", "BIC0001", "12345", AddressDto);

        CreateAccountDto originatorAccount =
            new CreateAccountDto("OriginatorAccount", "GB12FINA1234567891", "BIC0001", "12345", AddressDto);

        var validCreatePaymentDto = new CreatePaymentDto(originatorAccount, beneficiaryAccount, new MoneyDto(250.75m, "EUR"), DateTimeOffset.Now,
            ChargesBearer.Shared, "Initial payment details", "NEWREF123");

        var cancellationToken = CancellationToken.None;
        var createdPaymentDto = await _paymentService.CreatePaymentAsync(validCreatePaymentDto, cancellationToken);

        ClassicAssert.NotNull(createdPaymentDto);
        ClassicAssert.That(createdPaymentDto.Id, Is.GreaterThan(0));

        ClassicAssert.AreEqual(validCreatePaymentDto.ReferenceNumber, createdPaymentDto.ReferenceNumber);
        ClassicAssert.AreEqual(validCreatePaymentDto.Amount, createdPaymentDto.Amount);

        var paymentInDb = await _dbContext.Payments.FindAsync([createdPaymentDto.Id], cancellationToken);
        ClassicAssert.NotNull(paymentInDb);
        ClassicAssert.AreEqual(validCreatePaymentDto.ReferenceNumber, paymentInDb.ReferenceNumber);
    }

    [Test]
    public async Task CreatePaymentAsync_ShouldThrowArgumentNullException_WhenValidationFails()
    {
        var invalidCreatePaymentDto = new CreatePaymentDto(null, null, new MoneyDto(250.75m, "EUR"), DateTimeOffset.Now,
            ChargesBearer.Shared, "NEWREF123", null);

        var cancellationToken = CancellationToken.None;

        var thrownException = Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _paymentService.CreatePaymentAsync(invalidCreatePaymentDto, cancellationToken)
        );

        ClassicAssert.That(thrownException.Message, Contains.Substring("Payment reference number cannot be null or empty. (Parameter 'ReferenceNumber')"));

        var paymentCountAfter = await _dbContext.Payments.CountAsync(cancellationToken);
        ClassicAssert.AreEqual(0, paymentCountAfter);
    }
}
