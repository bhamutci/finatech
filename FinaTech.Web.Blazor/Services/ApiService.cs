namespace FinaTech.Web.Blazor.Services;

using Model.Response;


/// <summary>
/// Provides utility methods for making API requests over HTTP.
/// </summary>
public class ApiService: IApiService
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
    /// Sends a GET request to the specified API endpoint and returns the deserialized response data.
    /// </summary>
    /// <typeparam name="TResponse">The type of the data expected in the API response.</typeparam>
    /// <param name="endpoint">The URL of the API endpoint to send the GET request to.</param>
    /// <returns>A task representing the asynchronous operation, containing an
    /// <see cref="ApiResponse{TResponse}"/> object with the API response data or error information.</returns>
    public async Task<ApiResponse<TResponse>> GetAsync<TResponse>(string endpoint)
    {
        try
        {
            var response = await _httpClient.GetAsync(endpoint);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<TResponse>();
                return ApiResponse<TResponse>.Success(data);
            }
            else
            {
                return ApiResponse<TResponse>.Error($"Request failed with status code: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            return ApiResponse<TResponse>.Error($"An error occurred: {ex.Message}");
        }

    }

    /// <summary>
    /// Sends a POST request to the specified API endpoint with the provided data and
    /// deserializes the response to the specified type.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request data to be sent in the POST body.</typeparam>
    /// <typeparam name="TResponse">The type of the expected response data.</typeparam>
    /// <param name="endpoint">The API endpoint to which the POST request will be sent.</param>
    /// <param name="data">The data to be sent in the body of the POST request.</param>
    /// <returns>A task that represents the asynchronous operation.
    /// The task result contains an ApiResponse wrapping the response data or an error message.</returns>
    public async Task<ApiResponse<TResponse>> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(endpoint, data);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<TResponse>();
                return ApiResponse<TResponse>.Success(result);
            }
            else
            {
                return ApiResponse<TResponse>.Error($"Request failed with status code: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            return ApiResponse<TResponse>.Error($"An error occurred: {ex.Message}");
        }

    }
}
