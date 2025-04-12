using System.Net.Http.Json;

namespace Frontend.Services.Api;

public interface IApiService
{
    Task<TResponse> GetAsync<TResponse>(string uri);
    Task<TResponse> PostAsync<TResponse, TRequest>(string uri, TRequest request);
    Task<TResponse> PutAsync<TResponse, TRequest>(string uri, TRequest request);
    Task DeleteAsync(string uri);
    void SetToken(string token);
    void ClearToken();
} 