namespace FinaTech.Application.Exceptions;

public class AccountException: Exception
{
    protected AccountException(string message) : base(message) { }
    public AccountException(string message, Exception innerException) : base(message, innerException) { }
}
public class AccountAlreadyExistsException : BankException
{
    public AccountAlreadyExistsException(string message) : base(message) { }
    public AccountAlreadyExistsException(string message, Exception innerException) : base(message, innerException) { }
}
