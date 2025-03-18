using CartService.Models;
using Grpc.Net.Client;
using ProductService;

namespace CartService.DataServices.Grpc;

public class ProductDataClient : IProductDataClient
{
    private readonly IConfiguration _configuration;

    public ProductDataClient(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Product GetProductById(int id)
    {
        var channel = GrpcChannel.ForAddress(_configuration["GrpcConfigs:ProductServiceUrl"]);
        var client = new GrpcProduct.GrpcProductClient(channel);
        try
        {
            var response = client.GetProduct(new ProductRequest { Id = id });

            if (response.Product is null)
            {
                throw new ArgumentException("Invalid product id");
            }

            return new Product
            {
                Id = response.Product.Id,
                Name = response.Product.Name,
                Price = response.Product.Price,
                Description = response.Product.Description
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"gRPC error: {ex.Message}");
            return null;
        }

    }

    public IEnumerable<Product> GetProductsByUserId(int userId)
    {
        var channel = GrpcChannel.ForAddress(_configuration["GrpcConfigs:ProductServiceUrl"]);
        var client = new GrpcProduct.GrpcProductClient(channel);
        var response = client.GetUserProducts(new UserProductRequest { UserId = userId });

        var products = new List<Product>();
        foreach(var product in response.Products)
        {
            products.Add(new Product
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Description = product.Description
            });
        }

        return products;
    }
}
