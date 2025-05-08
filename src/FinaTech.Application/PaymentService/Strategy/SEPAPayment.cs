using FinaTech.Application.PaymentService.Dto;

namespace FinaTech.Application.PaymentService;

public class SEPAPayment(PaymentDto payment) : PaymentStrategy(payment)
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
