namespace FinaTech.Application.Exceptions;

public class BankException : Exception
{
    protected BankException(string message) : base(message) { }
    public BankException(string message, Exception innerException) : base(message, innerException) { }
}
public class BankAlreadyExistsException : BankException
{
    public BankAlreadyExistsException(string message) : base(message) { }
    public BankAlreadyExistsException(string message, Exception innerException) : base(message, innerException) { }
}
