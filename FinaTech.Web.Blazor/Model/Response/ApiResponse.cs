namespace FinaTech.Web.Blazor.Model.Response;

/// <summary>
/// Represents a standardized API response format that wraps the result data with status information.
/// </summary>
/// <typeparam name="T">The type of data contained in the response.</typeparam>
public class ApiResponse<T>
{
    /// <summary>
    /// The data returned from the API call.
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// The status of the API call, either "success" or "error".
    /// </summary>
    public string Status { get; set; } = "success";

    /// <summary>
    /// A message providing additional information about the result.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Creates a successful response with the provided data.
    /// </summary>
    public static ApiResponse<T> Success(T data, string message = "Operation completed successfully")
    {
        return new ApiResponse<T>
        {
            Data = data,
            Status = "success",
            Message = message
        };
    }

    /// <summary>
    /// Creates an error response with the provided message.
    /// </summary>
    public static ApiResponse<T> Error(string message)
    {
        return new ApiResponse<T>
        {
            Data = default,
            Status = "error",
            Message = message
        };
    }
}
