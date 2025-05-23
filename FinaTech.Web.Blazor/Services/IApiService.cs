using FinaTech.Web.Blazor.Model.Response;

namespace FinaTech.Web.Blazor.Services;

/// <summary>
/// Provides methods for interacting with an API service, including capabilities for sending GET and POST requests.
/// </summary>
public interface IApiService
{
    /// <summary>
    /// Sends an asynchronous GET request to the specified API endpoint and retrieves the response.
    /// </summary>
    /// <typeparam name="TResponse">The type of the data expected in the API response.</typeparam>
    /// <param name="endpoint">The endpoint URL of the API to send the GET request to.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an instance of
    /// <see cref="ApiResponse{T}"/> with the retrieved data or error information.</returns>
    Task<ApiResponse<TResponse>> GetAsync<TResponse>(string endpoint);

    /// <summary>
    /// Sends an asynchronous POST request to the specified API endpoint with the given data
    /// and retrieves the response.
    /// </summary>
    /// <typeparam name="TRequest">The type of the data being sent in the POST request.</typeparam>
    /// <typeparam name="TResponse">The type of the data expected in the API response.</typeparam>
    /// <param name="endpoint">The endpoint URL of the API to send the POST request to.</param>
    /// <param name="data">The data to be included in the body of the POST request.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an instance of
    /// <see cref="ApiResponse{TResponse}"/> with the retrieved data or error information.</returns>
    Task<ApiResponse<TResponse>> PostAsync<TRequest, TResponse>(string endpoint, TRequest data);
}
