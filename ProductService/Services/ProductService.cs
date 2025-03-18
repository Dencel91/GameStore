using AutoMapper;
using ProductService.Data;
using ProductService.Models;

namespace ProductService.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepo _productRepo;
        private readonly IMapper _mapper;
        private readonly IProductToUserRepo _productToUserRepo;

        public ProductService(IProductRepo productRepo, IProductToUserRepo productToUserRepo, IMapper mapper)
        {
            _productRepo = productRepo;
            _mapper = mapper;
            _productToUserRepo = productToUserRepo;
        }

        public Task<IEnumerable<Product>> GetProductsByUserId(int userId)
        {
            return _productToUserRepo.GetProductsByUserId(userId);
        }

        public Task<Product> GetProductById(int id)
        {
            return _productRepo.GetProductById(id);
        }
    }
}
