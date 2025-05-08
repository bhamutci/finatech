namespace FinaTech.Application.Services.Strategy;

using Payment.Dto;

public class SWIFTPayment(PaymentDto payment) : PaymentStrategy(payment)
{
    protected override void Process()
    {
        throw new NotImplementedException();
    }

    protected override bool Validate()
    {
        throw new NotImplementedException();
    }
}
