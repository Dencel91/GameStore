using AutoMapper;
using UserService.Dtos;
using Grpc.Net.Client;
using ProductService;
using UserService.Profiles;

namespace UserService.DataServices.Grpc;

public class ProductDataClient : IProductDataClient
{
    private readonly GrpcProduct.GrpcProductClient _client;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public ProductDataClient(GrpcProduct.GrpcProductClient client, IMapper mapper, ILogger<ProductDataClient> logger)
    {
        _client = client;
        _mapper = mapper;
        _logger = logger;
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
