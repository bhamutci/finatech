namespace FinaTech.Application.Services.Bank;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using EntityFramework.PostgresSqlServer;

using Dto;

/// <summary>
/// Represents operations for managing and retrieving bank-related information within the application.
/// </summary>
public class BankService : BaseApplicationService, IBankService
{
    /// <summary>
    /// Provides functionalities for managing banks, including retrieving, creating,
    /// and handling related operations within the banking domain.
    /// </summary>
    public BankService(FinaTechPostgresSqlDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
    {}

    /// <summary>
    /// Retrieves a specific bank's details based on the provided identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the bank to be retrieved.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the bank details encapsulated in a <see cref="BankDto"/> object.</returns>
    public async Task<BankDto> GetBankAsync(int id)
    {
        var bankEntity = await dbContext.Banks.FindAsync(id);
        return mapper.Map<BankDto>(bankEntity);
    }

    /// <summary>
    /// Asynchronously retrieves a collection of banks from the database
    /// and returns them as a read-only list of bank data transfer objects (DTOs).
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.
    /// The task result contains a read-only list of <see cref="BankDto"/> objects representing the banks.</returns>
    public async Task<IReadOnlyList<BankDto>> GetBanksAsync()
    {
        var bankEntities = await dbContext.Banks.ToListAsync();
        return mapper.Map<IReadOnlyList<BankDto>>(bankEntities);
    }

    public Task<BankDto> CreateBankAsync(BankDto bank)
    {
        throw new NotImplementedException();
    }
}
