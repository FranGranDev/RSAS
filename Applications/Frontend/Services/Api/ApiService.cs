using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;

namespace Frontend.Services.Api;

public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private string _token;

    public ApiService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClientFactory.CreateClient("ApiClient");
        _httpContextAccessor = httpContextAccessor;

        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<TResponse> GetAsync<TResponse>(string uri)
    {
        await SetAuthHeader();
        var response = await _httpClient.GetAsync(uri);
        return await HandleResponse<TResponse>(response);
    }

    public async Task<TResponse> PostAsync<TResponse, TRequest>(string uri, TRequest request)
    {
        await SetAuthHeader();
        var content = new StringContent(JsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(uri, content);
        return await HandleResponse<TResponse>(response);
    }

    public async Task<TResponse> PutAsync<TResponse, TRequest>(string uri, TRequest request)
    {
        await SetAuthHeader();
        var content = new StringContent(JsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync(uri, content);
        return await HandleResponse<TResponse>(response);
    }

    public async Task DeleteAsync(string uri)
    {
        await SetAuthHeader();
        var response = await _httpClient.DeleteAsync(uri);
        await HandleResponse<object>(response);
    }

    public void SetToken(string token)
    {
        _token = token;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        _httpContextAccessor.HttpContext?.Response.Cookies.Append("auth_token", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.Now.AddDays(7)
        });
    }

    public void ClearToken()
    {
        _token = null;
        _httpClient.DefaultRequestHeaders.Authorization = null;
        _httpContextAccessor.HttpContext?.Response.Cookies.Delete("auth_token");
    }

    private async Task SetAuthHeader()
    {
        var token = await _httpContextAccessor.HttpContext?.GetTokenAsync("Token");
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }

    private async Task<T> HandleResponse<T>(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        
        if (!response.IsSuccessStatusCode)
        {
            try
            {
                var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                throw new HttpRequestException(errorResponse?.Error ?? $"API request failed with status code {response.StatusCode}");
            }
            catch (JsonException)
            {
                throw new HttpRequestException($"API request failed with status code {response.StatusCode}. Response: {content}");
            }
        }

        if (typeof(T) == typeof(object))
        {
            return default;
        }

        try
        {
            return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch (JsonException ex)
        {
            throw new HttpRequestException($"Failed to deserialize response: {content}. Error: {ex.Message}");
        }
    }

    private class ErrorResponse
    {
        public string Error { get; set; }
    }
}  