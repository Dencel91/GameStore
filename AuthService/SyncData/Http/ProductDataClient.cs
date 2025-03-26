
using System.Text.Json;
using AuthService.Models;

namespace AuthService.SyncData.Http;

public class ProductDataClient : IProductDataClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public ProductDataClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<IEnumerable<Product>> GetProductsByUserId(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_configuration["ProductService"]}/GetProductsByUserId/{id}");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Cannot get user's products");
                return new List<Product>();
            }

            var responseContent = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrEmpty(responseContent))
            {
                throw new Exception("Cannot get user's products");
            }

            var deserialized = JsonSerializer.Deserialize<IEnumerable<Product>>(responseContent);

            if (deserialized == null)
            {
                throw new Exception("Cannot get user's products");
            }

            return deserialized;
        }
        catch
        {
            Console.WriteLine("Cannot get user's products");
            return new List<Product>();
        }
    }
}
