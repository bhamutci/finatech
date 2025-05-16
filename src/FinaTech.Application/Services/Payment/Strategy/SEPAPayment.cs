namespace FinaTech.Application.Services.Strategy;

using Payment.Dto;


public class SEPAPayment(Payment payment) : PaymentStrategy(payment)
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
