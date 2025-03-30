using AutoMapper;
using Grpc.Core;
using ProductService.Services;

namespace ProductService.DataServices.Grpc;

public class GrpcProductService : GrpcProduct.GrpcProductBase
{
    private readonly IMapper _mapper;
    private readonly IProductService _productService;

    public GrpcProductService(IProductService productService, IMapper mapper)
    {
        _mapper = mapper;
        _productService = productService;
    }

    public override async Task<GrpcProductResponse> GetProduct(GrpcProductRequest request, ServerCallContext context)
    {
        var product = await _productService.GetProduct(request.Id);
        var productResponse = new GrpcProductResponse() { Product = _mapper.Map<GrpcProductModel>(product) };
        return productResponse;
    }

    public override async Task<GetProductsByIdsGrpcResponse> GetProductsByIds(GetProductsByIdsGrpcRequest request, ServerCallContext context)
    {
        var products = await _productService.GetProductsByIds(request.Ids);
        var response = new GetProductsByIdsGrpcResponse();
        response.Products.AddRange(_mapper.Map<IEnumerable<GrpcProductModel>>(products));
        return response;
    }
}
