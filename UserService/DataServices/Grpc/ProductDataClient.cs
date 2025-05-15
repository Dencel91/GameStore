using AutoMapper;
using UserService.Dtos;
using ProductService;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace UserService.DataServices.Grpc;

public class ProductDataClient : IProductDataClient
{
    private readonly GrpcProduct.GrpcProductClient _client;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;
    private readonly IDistributedCache _cache;

    public ProductDataClient(
        GrpcProduct.GrpcProductClient client,
        IMapper mapper,
        ILogger<ProductDataClient> logger,
        IDistributedCache cache)
    {
        _client = client;
        _mapper = mapper;
        _logger = logger;
        _cache = cache;
    }

    public ProductDto GetProductById(int id)
    {
        try
        {
            var cachedProduct = _cache.GetString($"product-{id}");

            if (string.IsNullOrEmpty(cachedProduct))
            {
                var response = _client.GetProduct(new GrpcProductRequest { Id = id });

                if (response.Product is null)
                {
                    throw new ArgumentException("Invalid product id");
                }

                return _mapper.Map<ProductDto>(response.Product);
            }

            return JsonSerializer.Deserialize<ProductDto>(cachedProduct);

        }
        catch (Exception ex)
        {
            _logger.LogError("gRPC error in {methodName}: {message}", nameof(GetProductById), ex.Message);
            throw;
        }
    }

    public IEnumerable<ProductDto> GetProductsByIds(IEnumerable<int> ids)
    {
        try
        {
            var request = new GetProductsByIdsGrpcRequest();
            request.Ids.AddRange(ids);
            var response = _client.GetProductsByIds(request);

            var products = _mapper.Map<IEnumerable<ProductDto>>(response.Products);

            return products;
        }
        catch (Exception ex)
        {
            _logger.LogError("gRPC error in {methodName}: {message}", nameof(GetProductsByIds), ex.Message);
            throw;
        }
    }
}
