using AutoMapper;
using Microsoft.Extensions.Caching.Distributed;
using ProductService.Data;
using ProductService.DTOs;
using ProductService.Models;
using ProductService.Models.enums;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProductService.Services;

public class ProductService : IProductService
{
    const int DefaultPageSize = 12;

    private readonly IProductRepo _productRepo;
    private readonly IProductReviewRepo _productReviewRepo;
    private readonly IMapper _mapper;
    private readonly Lazy<IFileService> _fileService;
    private readonly AppDbContext _dbContext;
    private readonly IDistributedCache _cache;

    private static readonly JsonSerializerOptions serializerOptions = new JsonSerializerOptions
    {
        ReferenceHandler = ReferenceHandler.IgnoreCycles
    };

public ProductService(
        IProductRepo productRepo,
        IProductReviewRepo productReviewRepo,
        IMapper mapper,
        Lazy<IFileService> fileService,
        AppDbContext dbContext,
        IDistributedCache cache)
    {
        _productRepo = productRepo;
        _productReviewRepo = productReviewRepo;
        _mapper = mapper;
        _fileService = fileService;
        _dbContext = dbContext;
        _cache = cache;
    }

    public async Task<GetProductsResponse> GetPagedProducts(int pageCursor, int pageSize)
    {
        if (pageSize <= 0)
        {
            pageSize = DefaultPageSize;
        }

        var key = $"product-page-{pageCursor}-{pageSize}";

        var cachedProductPage = await _cache.GetStringAsync(key);

        IEnumerable<Product> products;

        if (string.IsNullOrEmpty(cachedProductPage))
        {
            products = await _productRepo.GetPagedProducts(pageCursor, pageSize);

            var response = new GetProductsResponse
            {
                Products = _mapper.Map<IEnumerable<ProductDto>>(products),
                NextPageCursor = products.Any() ? products.Last().Id : 0
            };

            await _cache.SetStringAsync(
                key,
                JsonSerializer.Serialize(response),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                });

            return response;
        }
        
        return JsonSerializer.Deserialize<GetProductsResponse>(cachedProductPage);
    }

    public async Task<Product?> GetProduct(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentException("Invalid product id");
        }

        var key = $"product-{id}";

        var cachedProduct = await _cache.GetStringAsync(key);

        if (string.IsNullOrEmpty(cachedProduct))
        {
            var product = await _productRepo.GetProduct(id);

            if (product is null)
            {
                return null;
            }

            await _cache.SetStringAsync(
                key,
                JsonSerializer.Serialize(product),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                });

            return product;
        }

        return JsonSerializer.Deserialize<Product>(cachedProduct);
    }

    public async Task<IEnumerable<Product>> SearchProduct(string searchText)
    {
        if (string.IsNullOrEmpty(searchText))
        {
            throw new ArgumentNullException(nameof(searchText));
        }

        var products = await _productRepo.SearchProduct(searchText);
        return products;
    }

    public async Task<Product> CreateProduct(CreateProductRequest request)
    {
        ValidateCreateProductRequest(request);

        var uploadedFiles = new List<ProductImage>();

        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var product = _mapper.Map<Product>(request);
            await _productRepo.CreateProduct(product);
            await _productRepo.SaveChanges();

            var thumbnail = await UploadProductImage(request.Thumbnail!, product.Id, ImageType.Thumbnail);
            product.ThumbnailUrl = thumbnail.Url;
            uploadedFiles.Add(thumbnail);

            foreach (var image in request.Images)
            {
                var productImage = await UploadProductImage(image, product.Id, ImageType.Preview);
                uploadedFiles.Add(productImage);
            }

            await _dbContext.ProductImages.AddRangeAsync(uploadedFiles);
            await _productRepo.SaveChanges();
            await transaction.CommitAsync();
            return product;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();

            foreach (var file in uploadedFiles)
            {
                await _fileService.Value.Delete(file.FullPath);
            }

            throw;
        }
    }

    private static void ValidateCreateProductRequest(CreateProductRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.Price < 0)
        {
            throw new ArgumentException("Price cannot be negative");
        }

        if (request.Images.Count() < 3)
        {
            throw new ArgumentException("At least 3 preview images are required");
        }
    }

    private async Task<ProductImage> UploadProductImage(IFormFile image, int productId, ImageType imageType)
    {
        var imageId = Guid.NewGuid();
        var filename = imageId.ToString() + Path.GetExtension(image.FileName);
        var fullPath = Path.Combine(productId.ToString(), filename);

        var imageUri = await _fileService.Value.Upload(fullPath, image);

        return new ProductImage
        {
            Id = imageId,
            Name = filename,
            FullPath = fullPath,
            Type = imageType,
            Url = imageUri,
            ProductId = productId
        };
    }

    public async Task<Product> UpdateProduct(UpdateProductRequest request)
    {
        ValidateUpdateProductRequest(request);

        var uploadedFiles = new List<ProductImage>();

        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var product = await _productRepo.GetProductDetails(request.ProductId)
                ?? throw new ArgumentException("Product not found");

            product.Name = request.Name;
            product.Description = request.Description;
            product.Price = request.Price;

            foreach (var image in request.NewImages)
            {
                var productImage = await UploadProductImage(image, product.Id, ImageType.Preview);
                uploadedFiles.Add(productImage);
            }

            var imagesToRemove = new List<ProductImage>();
            foreach (var imageUrl in request.RemovedImages)
            {
                var image = product.Images.First(i => i.Url == imageUrl);
                imagesToRemove.Add(image);
                product.Images.Remove(image);
            }

            if (request.UpdatedThumbnail != null)
            {
                var existedThumbnail = product.Images.FirstOrDefault(i => i.Type == ImageType.Thumbnail);

                if (existedThumbnail != null)
                {
                    imagesToRemove.Add(existedThumbnail);
                    product.Images.Remove(existedThumbnail);
                }

                var newThumbnail = await UploadProductImage(request.UpdatedThumbnail, product.Id, ImageType.Thumbnail);
                product.ThumbnailUrl = newThumbnail.Url;
                uploadedFiles.Add(newThumbnail);
            }

            await _dbContext.ProductImages.AddRangeAsync(uploadedFiles);

            await _productRepo.SaveChanges();

            foreach (var image in imagesToRemove)
            {
                await _fileService.Value.Delete(image.FullPath);
            }

            await transaction.CommitAsync();
            return product;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();

            foreach (var file in uploadedFiles)
            {
                await _fileService.Value.Delete(file.FullPath);
            }

            throw;
        }
    }

    private void ValidateUpdateProductRequest(UpdateProductRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.ProductId <= 0)
        {
            throw new ArgumentException("Invalid product id");
        }

        if (request.Price < 0)
        {
            throw new ArgumentException("Price cannot be negative");
        }
    }

    public Task<IEnumerable<Product>> GetProductsByIds(IEnumerable<int> ids)
    {
        return _productRepo.GetProductsByIds(ids);
    }

    public async Task<Product?> GetProductDetails(int id)
    {
        var key = $"product-detail-{id}";

        var cachedProduct = await _cache.GetStringAsync(key);

        if (string.IsNullOrEmpty(cachedProduct))
        {
            var product = await _productRepo.GetProductDetails(id);

            if (product is null)
            {
                return null;
            }

            product.Reviews = await _productReviewRepo.GetReviewsByProductId(id);

            await _cache.SetStringAsync(
                key,
                JsonSerializer.Serialize(product, serializerOptions),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                });

            return product;
        }

        return JsonSerializer.Deserialize<Product>(cachedProduct);
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
