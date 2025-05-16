namespace FinaTech.Tests.Services;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using FluentValidation;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using AutoMapper;

using Core;
using Core.Account;
using Application.Mapper;
using EntityFramework.PostgresSqlServer;
using FinaTech.Application.Services.Payment;
using FinaTech.Application.Services.Payment.Dto;
using FinaTech.Application.Services.Payment.Dto.Validator;

[TestFixture]
public class PaymentServiceTests
{
    private FinaTechPostgresSqlDbContext _dbContext;
    private IPaymentService _paymentService;
    private static readonly CreateAddressDto BeneficiaryAccountAddress = new("Test St",string.Empty, string.Empty, string.Empty, string.Empty, CountryCode:"GB");
    private static readonly CreateAddressDto OriginatorAccountAddress = new("Test St",string.Empty, string.Empty, string.Empty, string.Empty, CountryCode:"GB");

    private static readonly CreateAccountDto BeneficiaryAccount = new CreateAccountDto
        ("BeneficiaryAccount", "GB12FINA1234567890", "BIC0001", "12344", BeneficiaryAccountAddress);
    private static readonly CreateAccountDto OriginatorAccount =
        new("OriginatorAccount", "GB12FINA1234567891", "BIC0001", "12345", OriginatorAccountAddress);

    private ServiceProvider _serviceProvider;

    [SetUp]
    public void Setup()
    {
        _serviceProvider = new ServiceCollection()
            .AddLogging()
            .AddAutoMapper(typeof(DtoAutoMapperProfile).Assembly)
            .AddDbContext<FinaTechPostgresSqlDbContext>(options => options.UseInMemoryDatabase("ServicesDB"))
            .AddScoped<IValidator<CreateAccountDto>, CreateAccountDtoValidator>()
            .AddScoped<IValidator<CreatePaymentDto>, CreatePaymentDtoValidator>()
            .AddScoped<IValidator<CreateAddressDto>, AddressDtoValidator>()
            .AddScoped<IPaymentService, PaymentService>()
            .BuildServiceProvider();

        _dbContext = _serviceProvider.GetService<FinaTechPostgresSqlDbContext>();
        _paymentService = _serviceProvider.GetService<IPaymentService>();

    }

    [TearDown]
    public void TearDown()
    {
        _dbContext?.Database.EnsureDeleted();
        _dbContext?.Dispose();
        _serviceProvider.Dispose();
    }

    [Test]
    public async Task GetPaymentAsync_ShouldReturnPaymentDto_WhenPaymentFound()
    {
        var paymentId = 50;
        var samplePayment = GetSamplePayments(1, paymentId).Single();

        _dbContext.Payments.Add(samplePayment);
        await _dbContext.SaveChangesAsync();

        var cancellationToken = CancellationToken.None;

        var resultDto = await _paymentService.GetPaymentAsync(paymentId, cancellationToken);

        Assert.That(resultDto, Is.Not.Null);
        Assert.That(resultDto.Id, Is.EqualTo(paymentId));
        Assert.That(resultDto.ReferenceNumber, Is.EqualTo(samplePayment.ReferenceNumber));
        Assert.That(resultDto.Amount.Value, Is.EqualTo(samplePayment.Amount.Value));
        Assert.That(resultDto.Amount.Currency, Is.EqualTo(samplePayment.Amount.Currency));
    }

    [Test]
    public async Task GetPaymentAsync_ShouldReturnNull_WhenPaymentNotFound()
    {
        var nonExistentPaymentId = 999;
        var cancellationToken = CancellationToken.None;

        var result = await _paymentService.GetPaymentAsync(nonExistentPaymentId, cancellationToken);
        Assert.That(result, Is.Null);
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

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Items, Is.Not.Null);

        Assert.That(result.TotalCount, Is.EqualTo(totalNumberOfPayments));
        Assert.That(result.Items.Count, Is.EqualTo(maxResultCount));

        var expectedIdsInPage = samplePayments.Skip(skipCount).Take(maxResultCount).Select(p => p.Id).ToList();
        var actualIdsInPage = result.Items.Select(dto => dto.Id).ToList();

        Assert.That(actualIdsInPage, Is.EquivalentTo(expectedIdsInPage));
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
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Items, Is.Not.Null);

        Assert.That(result.TotalCount, Is.EqualTo(expectedFilteredCount));
        Assert.That(result.Items.Count, Is.EqualTo(expectedFilteredCount));

        var expectedFilteredIds = expectedFilteredPayments.Select(p => p.Id).ToList();
        var actualFilteredIds = result.Items.Select(dto => dto.Id).ToList();
        Assert.That(actualFilteredIds, Is.EquivalentTo(expectedFilteredIds));
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
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Items, Is.Not.Null);
        Assert.That(result.TotalCount, Is.EqualTo(0));
        Assert.That(result.Items.Count, Is.EqualTo(0));

    }

    [Test]
    public async Task CreatePaymentAsync_ShouldCreatePayment_WhenValidInput()
    {
        var validCreatePaymentDto = new CreatePaymentDto(OriginatorAccount, BeneficiaryAccount, new MoneyDto(250.75m, "EUR"), DateTimeOffset.Now,
            ChargesBearer.Shared, "Initial payment details", "NEWREF123");

        var cancellationToken = CancellationToken.None;
        var createdPaymentDto = await _paymentService.CreatePaymentAsync(validCreatePaymentDto, cancellationToken);

        ClassicAssert.NotNull(createdPaymentDto);
        ClassicAssert.That(createdPaymentDto.Id, Is.GreaterThan(0));

        Assert.That(createdPaymentDto.ReferenceNumber, Is.EqualTo(validCreatePaymentDto.ReferenceNumber));
        Assert.That(createdPaymentDto.Amount, Is.EqualTo(validCreatePaymentDto.Amount));

        var paymentInDb = await _dbContext.Payments.FindAsync([createdPaymentDto.Id], cancellationToken);
        Assert.That(paymentInDb, Is.Not.Null);
        Assert.That(paymentInDb?.ReferenceNumber, Is.EqualTo(validCreatePaymentDto.ReferenceNumber));
    }

    [Test]
    public async Task CreatePaymentAsync_ShouldThrowValidationException_WhenValidationFails()
    {
        var invalidCreatePaymentDto = new CreatePaymentDto(null, BeneficiaryAccount, new MoneyDto(250.75m, "EUR"), DateTimeOffset.Now,
            ChargesBearer.Shared, "NEWREF123", null);

        var cancellationToken = CancellationToken.None;

        var thrownException = Assert.ThrowsAsync<Application.Exceptions.ValidationException>(async () =>
            await _paymentService.CreatePaymentAsync(invalidCreatePaymentDto, cancellationToken)
        );

        Assert.That(thrownException.Message, Contains.Substring("Payment input validation failed."));

        var paymentCountAfter = await _dbContext.Payments.CountAsync(cancellationToken);
        Assert.That(paymentCountAfter, Is.EqualTo(0));
    }

    private List<Payment> GetSamplePayments(int count, int startId = 1)
    {
        var payments = new List<Payment>();
        var mapper = _serviceProvider.GetService<IMapper>();
        for (int i = 0; i < count; i++)
        {
            var id = startId + i;
            payments.Add(new Payment
            {
                Id = id,
                ReferenceNumber = $"REF{id.ToString().PadLeft(5, '0')}",
                Details = $"Details for payment {id}",
                Amount = new Money(10,"GB"),
                Date = DateTimeOffset.Now,
                ChargesBearer = (int)ChargesBearer.Originator,
                OriginatorAccount =  mapper?.Map<CreateAccountDto, Account>(OriginatorAccount) ,
                BeneficiaryAccount = mapper?.Map<CreateAccountDto, Account>(BeneficiaryAccount)
            });
        }

        return payments;
    }
}
