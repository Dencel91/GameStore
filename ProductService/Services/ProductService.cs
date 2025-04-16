using AutoMapper;
using ProductService.Data;
using ProductService.DTOs;
using ProductService.Models;
using ProductService.Models.enums;

namespace ProductService.Services;

public class ProductService : IProductService
{
    const int DefaultPageSize = 12;

    private readonly IProductRepo _productRepo;
    private readonly IProductReviewRepo _productReviewRepo;
    private readonly IMapper _mapper;
    private readonly Lazy<IFileService> _fileService;
    private readonly AppDbContext _dbContext;

    public ProductService(
        IProductRepo productRepo,
        IProductReviewRepo productReviewRepo,
        IMapper mapper,
        Lazy<IFileService> fileService,
        AppDbContext dbContext)
    {
        _productRepo = productRepo;
        _productReviewRepo = productReviewRepo;
        _mapper = mapper;
        _fileService = fileService;
        _dbContext = dbContext;
    }

    public async Task<GetProductsResponse> GetPagedProducts(int pageCursor, int pageSize)
    {
        if (pageSize <= 0)
        {
            pageSize = DefaultPageSize;
        }

        var products = await _productRepo.GetPagedProducts(pageCursor, pageSize);

        var response = new GetProductsResponse
        {
            Products = _mapper.Map<IEnumerable<ProductDto>>(products),
            NextPageCursor = products.Any() ? products.Last().Id : 0
        };

        return response;
    }

    public Task<Product?> GetProduct(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentException("Invalid product id", nameof(id));
        }

        return _productRepo.GetProduct(id);
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
            throw new ArgumentException("Price cannot be negative", nameof(request));
        }

        if (request.Images.Count() < 3)
        {
            throw new ArgumentException("At least 3 preview images are required", nameof(request));
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
                ?? throw new ArgumentException("Product not found", nameof(request));

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
            throw new ArgumentException("Invalid product id", nameof(request));
        }

        if (request.Price < 0)
        {
            throw new ArgumentException("Price cannot be negative", nameof(request));
        }

        //if (request.Images.Count() < 3)
        //{
        //    throw new ArgumentException("At least 3 preview images are required", nameof(request));
        //}
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
