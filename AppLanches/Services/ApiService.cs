using AppLanches.Models;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace AppLanches.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl = "https://ng1w4xtm-44353.uks1.devtunnels.ms/";
    private readonly ILogger<ApiService> _logger;

    JsonSerializerOptions _serializerOptions;
    public ApiService(HttpClient httpClient,
                      ILogger<ApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _serializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<ApiResponse<bool>> RegisterUser(string name, string email,
                                                          string phone, string password)
    {
        try
        {
            var register = new Register()
            {
                Name = name,
                Email = email,
                Phone = phone,
                Password = password
            };

            var json = JsonSerializer.Serialize(register, _serializerOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await PostRequest("api/Users/Register", content);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Error sending HTTP request: {response.StatusCode}");
                return new ApiResponse<bool>
                {
                    ErrorMessage = $"Error sending HTTP request: {response.StatusCode}"
                };
            }

            return new ApiResponse<bool> { Data = true };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error registering user: {ex.Message}");
            return new ApiResponse<bool> { ErrorMessage = ex.Message };
        }
    }


    public async Task<ApiResponse<bool>> Login(string email, string password)
    {
        try
        {
            var login = new Login()
            {
                Email = email,
                Password = password
            };

            var json = JsonSerializer.Serialize(login, _serializerOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await PostRequest("api/Users/Login", content);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Error sending HTTP request: {response.StatusCode}");
                return new ApiResponse<bool>
                {
                    ErrorMessage = $"Error sending HTTP request: {response.StatusCode}"
                };
            }

            var jsonResult = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<Token>(jsonResult, _serializerOptions);

            Preferences.Set("AccessToken", result!.AccessToken);
            Preferences.Set("UserId", (int)result.UserId!);
            Preferences.Set("UserName", result.UserName);

            return new ApiResponse<bool> { Data = true };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro no login : {ex.Message}");
            return new ApiResponse<bool> { ErrorMessage = ex.Message };
        }
    }

    public async Task<(List<CartItem>? CartItems, string? ErrorMessage)> GetItemsCart(int userId)
    {
        var endpoint = $"api/ShoppingCartItems/{userId}";
        return await GetAsync<List<CartItem>>(endpoint);
    }

    public async Task<ApiResponse<bool>> AddItemToCart(Cart cart)
    {
        try
        {
            var json = JsonSerializer.Serialize(cart, _serializerOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await PostRequest("api/ShoppingCartItems", content);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Error in HTTP request: {response.StatusCode}");
                return new ApiResponse<bool>
                {
                    ErrorMessage = $"Error in HTTP request: {response.StatusCode}"
                };
            }

            return new ApiResponse<bool> { Data = true };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error adding item to cart: {ex.Message}");
            return new ApiResponse<bool> { ErrorMessage = ex.Message };
        }

    }

    private async Task<HttpResponseMessage> PostRequest(string uri, HttpContent content)
    {
        var url = _baseUrl + uri;
        try
        {
            var result = await _httpClient.PostAsync(url, content);
            return result;
        }
        catch (Exception ex)
        {
            // Log o erro ou trate conforme necessário
            _logger.LogError($"Error sending POST requesto to {uri}: {ex.Message}");
            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        }
    }

    public async Task<(List<Category>? Categories, string? ErrorMessage)> GetCategories()
    {
        return await GetAsync<List<Category>>("api/categories");
    }

    public async Task<(List<Product>? Products, string? ErrorMessage)> GetProducts(string productType, string categoryId)
    {
        //string endpoint = $"api/Products?tipoProduto={productType}&categoryId={categoryId}";
        string endpoint = $"api/Products?Search={productType}&categoryId={categoryId}";
        return await GetAsync<List<Product>>(endpoint);
    }

    private async Task<(T? Data, string? ErrorMessage)> GetAsync<T>(string endpoint)
    {
        try
        {
            AddAuthorizationHeader();

            var response = await _httpClient.GetAsync(AppConfig.BaseUrl + endpoint);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<T>(responseString, _serializerOptions);
                return (data ?? Activator.CreateInstance<T>(), null);
            }
            else
            {
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    string errorMessage = "Unauthorized";
                    _logger.LogWarning(errorMessage);
                    return (default, errorMessage);
                }

                string generalErrorMessage = $"Error in request: {response.ReasonPhrase}";
                _logger.LogError(generalErrorMessage);
                return (default, generalErrorMessage);
            }
        }
        catch (HttpRequestException ex)
        {
            string errorMessage = $"Error in HTTP request: {ex.Message}";
            _logger.LogError(ex, errorMessage);
            return (default, errorMessage);
        }
        catch (JsonException ex)
        {
            string errorMessage = $"Error in JSON desserialize: {ex.Message}";
            _logger.LogError(ex, errorMessage);
            return (default, errorMessage);
        }
        catch (Exception ex)
        {
            string errorMessage = $"Unexpected error: {ex.Message}";
            _logger.LogError(ex, errorMessage);
            return (default, errorMessage);
        }
    }

    private void AddAuthorizationHeader()
    {
        var token = Preferences.Get("AccessToken", string.Empty);
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task<(Product? Product, string? ErrorMessage)> GetProductDetails(int productId)
    {
        string endpoint = $"api/Products/{productId}";
        return await GetAsync<Product>(endpoint);
    }

    

}
