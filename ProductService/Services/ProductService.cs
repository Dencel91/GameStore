using AutoMapper;
using ProductService.Data;
using ProductService.DTOs;
using ProductService.Models;

namespace ProductService.Services;

public class ProductService : IProductService
{
    private readonly IProductRepo _productRepo;
    private readonly IProductImageRepo _productImageRepo;
    private readonly IProductReviewRepo _productReviewRepo;
    private readonly IMapper _mapper;

    public ProductService(
        IProductRepo productRepo,
        IProductImageRepo productImageRepo,
        IProductReviewRepo productReviewRepo,
        IMapper mapper)
    {
        _productRepo = productRepo;
        _productImageRepo = productImageRepo;
        _productReviewRepo = productReviewRepo;
        _mapper = mapper;
    }

    public async Task<IEnumerable<Product>> GetAllProducts()
    {
        var products = await _productRepo.GetAllProducts();
        return products;
    }

    public Task<Product?> GetProduct(int id)
    {
        ArgumentNullException.ThrowIfNull(id);

        return _productRepo.GetProduct(id);
    }

    public async Task<Product> CreateProduct(CreateProductRequest request)
    {
        ValidateCreateProductRequest(request);

        var product = _mapper.Map<Product>(request);

        await _productRepo.CreateProduct(product);
        await _productRepo.SaveChanges();

        return product;
    }

    private static void ValidateCreateProductRequest(CreateProductRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.Price < 0)
        {
            throw new ArgumentException("Price cannot be negative", nameof(request));
        }

        if (request.ImageUrls.Count() < 3)
        {
            throw new ArgumentException("At least 3 images are required", nameof(request));
        }
    }

    public Task<IEnumerable<Product>> GetProductsByIds(IEnumerable<int> ids)
    {
        return _productRepo.GetProductsByIds(ids);
    }

    public async Task<Product?> GetProductDetails(int id)
    {
        //var product = await _productRepo.GetProduct(id);
        var product = await _productRepo.GetProductDetails(id);

        if (product is null)
        {
            return null;
        }

        //var images = product.Images;

        //var getImagesTask = _productImageRepo.GetImagesByProductId(id);
        product.Reviews = await _productReviewRepo.GetReviewsByProductId(id);

        //Task.WaitAll([getImagesTask, getReviewsTask]);

        //product.Images = getImagesTask.Result;
        //product.Reviews = getReviewsTask.Result;

        return product;
    }

    public async Task<ProductReview> CreateProductReview(CreateProductReviewRequest request)
    {
        ValidateCreateProductReviewRequest(request);

        var review = _mapper.Map<ProductReview>(request);

        review.Date = new DateTimeOffset(DateTime.UtcNow, TimeSpan.Zero);

        await _productReviewRepo.CreateProductReview(review);
        await _productReviewRepo.SaveChanges();

        return review;
    }

    private void ValidateCreateProductReviewRequest(CreateProductReviewRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
    }
}
