namespace FinaTech.Application.Exceptions;

public class PaymentException : Exception
{
    protected PaymentException(string message) : base(message) { }
    public PaymentException(string message, Exception innerException) : base(message, innerException) { }
}
public class PaymentAlreadyExistsException : PaymentException
{
    public PaymentAlreadyExistsException(string message) : base(message) { }
    public PaymentAlreadyExistsException(string message, Exception innerException) : base(message, innerException) { }
}
