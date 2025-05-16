namespace FinaTech.Application.Exceptions;

public class ValidationException : Exception
{
    /// <summary>
    /// Gets the list of validation error messages associated with the exception.
    /// </summary>
    /// <remarks>
    /// This property provides a collection of validation error messages that describe why the validation failed.
    /// It is initialized during the construction of the <see cref="ValidationException"/> class and is never null.
    /// </remarks>
    public IReadOnlyList<string> Errors { get; }

    /// <summary>
    /// Represents an exception that occurs during validation processes.
    /// </summary>
    public ValidationException(string message, IReadOnlyList<string> errors) : base(message)
    {
        Errors = errors ?? new List<string>().AsReadOnly();
    }

    /// <summary>
    /// Represents an exception that is thrown when validation errors occur.
    /// </summary>
    public ValidationException(IReadOnlyList<string> errors) : this("Validation failed.", errors)
    {
    }

    /// <summary>
    /// Represents an exception thrown during validation processes.
    /// </summary>
    public ValidationException(string message, IReadOnlyList<string> errors, Exception innerException) : base(message,
        innerException)
    {
        Errors = errors ?? new List<string>().AsReadOnly();
    }
}
