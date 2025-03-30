using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductService.DTOs;
using ProductService.Services;

namespace ProductService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly IMapper _mapper;

    public ProductsController(IProductService productService, IMapper mapper)
    {
        _productService = productService;
        _mapper = mapper;
    }

    [HttpGet(Name = "GetProducts")]
    public async Task<ActionResult<IEnumerable<ProductResponse>>> GetProducts()
    {
        var products = await _productService.GetAllProducts();
        return base.Ok(_mapper.Map<IEnumerable<ProductResponse>>(products));
    }

    [HttpGet("{id}", Name = "GetProductById")]
    public async Task<ActionResult<ProductResponse>> GetProductById(int id)
    {
        var product = await _productService.GetProduct(id);

        return product is not null ? base.Ok(_mapper.Map<ProductResponse>(product)) : NotFound();
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<ProductResponse>> CreateProduct(CreateProductRequest createProductRequest)
    {
        var product = await _productService.CreateProduct(createProductRequest);
        var productResponse = _mapper.Map<ProductResponse>(product);

        return CreatedAtRoute(nameof(GetProductById), new { product.Id }, productResponse);
    }

    [HttpPost]
    [Route("GetProductsByIds")]
    public async Task<ActionResult<IEnumerable<ProductResponse>>> GetProductsByIds(IEnumerable<int> Ids)
    {
        var products = await _productService.GetProductsByIds(Ids);
        return base.Ok(_mapper.Map<IEnumerable<ProductResponse>>(products));
    }

    [HttpGet]
    [Route("GetProductsDetails/{id}")]
    public async Task<ActionResult<IEnumerable<ProductDetailsResponse>>> GetProductsDetails(int id)
    {
        var product = await _productService.GetProductDetails(id);
        return product is not null ? base.Ok(_mapper.Map<ProductDetailsResponse>(product)) : NotFound();
    }

    [HttpPost]
    [Route("AddReview")]
    public async Task<ActionResult> CreateProductReview(CreateProductReviewRequest request)
    {
        var product = await _productService.CreateProductReview(request);
        return base.Ok();
    }
}