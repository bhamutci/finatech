using FinaTech.Application.Payment.Dto;

namespace FinaTech.Application.Payment;

public class PaymentService: IPaymentService
{
    public Task<PaymentDto> GetPaymentAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<PaymentDto> CreatePaymentAsync(PaymentDto payment)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<PaymentDto>> GetPaymentsAsync()
    {
        throw new NotImplementedException();
    }
}
