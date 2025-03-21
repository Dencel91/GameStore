using AutoMapper;
using CartService.DTOs;
using Grpc.Net.Client;
using ProductService;

namespace CartService.DataServices.Grpc;

public class ProductDataClient : IProductDataClient
{
    private readonly GrpcProduct.GrpcProductClient _client;
    private readonly IMapper _mapper;

    public ProductDataClient(GrpcProduct.GrpcProductClient client, IMapper mapper)
    {
        _client = client;
        _mapper = mapper;
    }

    public ProductDto GetProductById(int id)
    {
        try
        {
            var response = _client.GetProduct(new GrpcProductRequest { Id = id });

            if (response.Product is null)
            {
                throw new ArgumentException("Invalid product id");
            }

            return _mapper.Map<ProductDto>(response.Product);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"gRPC error: {ex.Message}");
            throw;
        }

    }

    public IEnumerable<ProductDto> GetProductsByUserId(int userId)
    {
        var response = _client.GetUserProducts(new GrpcUserProductRequest { UserId = userId });
        var products = _mapper.Map<IEnumerable<ProductDto>>(response.Products);
        return products;
    }

    public IEnumerable<ProductDto> GetProductsByIds(IEnumerable<int> ids)
    {
        var request = new GetProductsByIdsGrpcRequest();
        request.Ids.AddRange(ids);
        var response = _client.GetProductsByIds(request);

        var products = _mapper.Map<IEnumerable<ProductDto>>(response.Products);

        return products;
    }
}
