
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using UserService.Models;

namespace UserService.SyncData.Http;

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
                throw new Exception("Cannot send product to user");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var deserialized = JsonSerializer.Deserialize<IEnumerable<Product>>(responseContent);

            return deserialized;
        }
        catch (Exception ex)
        {
            throw new Exception("Cannot send product to user", ex);
        }
    }
}
