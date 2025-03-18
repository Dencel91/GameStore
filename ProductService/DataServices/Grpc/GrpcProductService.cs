using AutoMapper;
using Grpc.Core;
using ProductService.Data;
using ProductService.Services;

namespace ProductService.DataServices.Grpc
{
    public class GrpcProductService : GrpcProduct.GrpcProductBase
    {
        private readonly IMapper _mapper;
        private readonly IProductService _productService;

        public GrpcProductService(IProductService productService, IMapper mapper)
        {
            _mapper = mapper;
            _productService = productService;
        }

        public override async Task<ProductResponse> GetProduct(ProductRequest request, ServerCallContext context)
        {
            var product = await _productService.GetProductById(request.Id);
            var productResponse = new ProductResponse() { Product = _mapper.Map<GrpcProductModel>(product) };
            return productResponse;
        }

        public override async Task<UserProductResponse> GetUserProducts(UserProductRequest request, ServerCallContext context)
        {
            var products = await _productService.GetProductsByUserId(request.UserId);
            var userProductResponse = new UserProductResponse();
            userProductResponse.Products.AddRange(_mapper.Map<IEnumerable<GrpcProductModel>>(products));
            return userProductResponse;
        }
    }
}
