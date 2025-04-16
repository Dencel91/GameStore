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
    public async Task<ActionResult<GetProductsResponse>> GetProducts(int nextPageCursor, int pageSize)
    {
        var response = await _productService.GetPagedProducts(nextPageCursor, pageSize);
        return base.Ok(response);
    }

    [HttpGet("{id}", Name = "GetProductById")]
    public async Task<ActionResult<ProductDto>> GetProductById(int id)
    {
        var product = await _productService.GetProduct(id);

        return product is not null ? base.Ok(_mapper.Map<ProductDto>(product)) : NotFound();
    }

    [HttpGet("GetProductsByIds")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByIds(IEnumerable<int> Ids)
    {
        var products = await _productService.GetProductsByIds(Ids);
        return base.Ok(_mapper.Map<IEnumerable<ProductDto>>(products));
    }

    [HttpGet("Search")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> SearchProduct(string searchText)
    {
        var products = await _productService.SearchProduct(searchText);
        return base.Ok(_mapper.Map<IEnumerable<ProductDto>>(products));
    }

    [HttpGet]
    [Route("GetProductsDetails/{id}")]
    public async Task<ActionResult<IEnumerable<ProductDetailsResponse>>> GetProductsDetails(int id)
    {
        var product = await _productService.GetProductDetails(id);
        return product is not null ? base.Ok(_mapper.Map<ProductDetailsResponse>(product)) : NotFound();
    }


    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<ProductDto>> CreateProduct(CreateProductRequest request)
    {
        var product = await _productService.CreateProduct(request);
        var productDto = _mapper.Map<ProductDto>(product);

        return CreatedAtRoute(nameof(GetProductById), new { product.Id }, productDto);
    }

    //[Authorize(Roles = "Admin")]
    [HttpPatch]
    public async Task<ActionResult<ProductDto>> UpdateProduct([FromForm] UpdateProductRequest request)
    {
        var product = await _productService.UpdateProduct(request);
        var productDto = _mapper.Map<ProductDto>(product);

        return Ok(productDto);
    }

    [HttpPost("AddReview")]
    public async Task<ActionResult> CreateProductReview(CreateProductReviewRequest request)
    {
        var product = await _productService.CreateProductReview(request);
        return base.Ok();
    }
}