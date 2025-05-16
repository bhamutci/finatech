namespace FinaTech.Web.Blazor.Service;

/// <summary>
/// Provides utility methods for making API requests over HTTP.
/// </summary>
public class ApiService
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Provides utility methods for consuming APIs via HTTP requests.
    /// Includes support for HTTP GET and POST requests with JSON serialization and deserialization.
    /// </summary>
    public ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Sends an HTTP GET request to the specified endpoint and deserializes the response into the specified type.
    /// </summary>
    /// <typeparam name="TResponse">The type to which the response will be deserialized.</typeparam>
    /// <param name="endpoint">The relative or absolute URI of the resource to retrieve.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the deserialized response of type <typeparamref name="TResponse"/>.</returns>
    public async Task<TResponse?> GetAsync<TResponse>(string endpoint)
    {
        var response = await _httpClient.GetFromJsonAsync<TResponse>(endpoint);
        return response;
    }

    /// <summary>
    /// Sends an HTTP POST request to the specified endpoint with the provided data
    /// and deserializes the response into the specified type.
    /// </summary>
    /// <typeparam name="TRequest">The type of the data to send in the request body.</typeparam>
    /// <typeparam name="TResponse">The type to which the response will be deserialized.</typeparam>
    /// <param name="endpoint">The relative or absolute URI of the API endpoint to send the request to.</param>
    /// <param name="data">The data to include in the request body.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the deserialized response of type <typeparamref name="TResponse"/>.</returns>
    public async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
    {
        var response = await _httpClient.PostAsJsonAsync(endpoint, data);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TResponse>();
    }

}
