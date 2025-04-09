using System.Net.Http.Json;
using System.Text.Json;

namespace Frontend.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<LoginResult> LoginAsync(string email, string password)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/auth/login", new { email, password });
                if (response.IsSuccessStatusCode)
                {
                    var token = await response.Content.ReadFromJsonAsync<TokenResponse>();
                    if (token != null)
                    {
                        // Сохраняем токен в куки
                        var cookieOptions = new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = true,
                            SameSite = SameSiteMode.Strict,
                            Expires = DateTimeOffset.UtcNow.AddHours(1)
                        };

                        _httpContextAccessor.HttpContext?.Response.Cookies.Append("JWT", token.Token, cookieOptions);
                        return LoginResult.Success;
                    }
                }
                return LoginResult.Failed("Неверный логин или пароль");
            }
            catch (Exception ex)
            {
                return LoginResult.Failed(ex.Message);
            }
        }

        public async Task LogoutAsync()
        {
            _httpContextAccessor.HttpContext?.Response.Cookies.Delete("JWT");
        }

        public async Task<List<OrderViewModel>> GetOrdersAsync()
        {
            try
            {
                var token = _httpContextAccessor.HttpContext?.Request.Cookies["JWT"];
                if (string.IsNullOrEmpty(token))
                {
                    throw new UnauthorizedAccessException("Требуется авторизация");
                }

                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.GetAsync("/api/orders");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<OrderViewModel>>();
            }
            catch (Exception ex)
            {
                // Здесь можно добавить логирование ошибок
                throw;
            }
        }

        public async Task<OrderViewModel> GetOrderAsync(int id)
        {
            try
            {
                var token = _httpContextAccessor.HttpContext?.Request.Cookies["JWT"];
                if (string.IsNullOrEmpty(token))
                {
                    throw new UnauthorizedAccessException("Требуется авторизация");
                }

                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.GetAsync($"/api/orders/{id}");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<OrderViewModel>();
            }
            catch (Exception ex)
            {
                // Здесь можно добавить логирование ошибок
                throw;
            }
        }

        public async Task<bool> CreateOrderAsync(OrderViewModel order)
        {
            try
            {
                var token = _httpContextAccessor.HttpContext?.Request.Cookies["JWT"];
                if (string.IsNullOrEmpty(token))
                {
                    throw new UnauthorizedAccessException("Требуется авторизация");
                }

                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.PostAsJsonAsync("/api/orders", order);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                // Здесь можно добавить логирование ошибок
                return false;
            }
        }

        public async Task<bool> UpdateOrderAsync(int id, OrderViewModel order)
        {
            try
            {
                var token = _httpContextAccessor.HttpContext?.Request.Cookies["JWT"];
                if (string.IsNullOrEmpty(token))
                {
                    throw new UnauthorizedAccessException("Требуется авторизация");
                }

                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.PutAsJsonAsync($"/api/orders/{id}", order);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                // Здесь можно добавить логирование ошибок
                return false;
            }
        }

        private class TokenResponse
        {
            public string Token { get; set; }
        }
    }

    public class LoginResult
    {
        public bool Succeeded { get; private set; }
        public string Error { get; private set; }

        private LoginResult(bool succeeded, string error = null)
        {
            Succeeded = succeeded;
            Error = error;
        }

        public static LoginResult Success => new LoginResult(true);
        public static LoginResult Failed(string error) => new LoginResult(false, error);
    }

    public class OrderViewModel
    {
        public int Id { get; set; }
        public string ClientName { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
    }
} 