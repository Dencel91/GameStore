using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProductService.Data;
using ProductService.DTOs;
using ProductService.Models;
using ProductService.Services;

namespace ProductService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IProductRepo _productRepo;
    private readonly IMapper _mapper;
    private readonly IProductToUserRepo _productToUserRepo;
    private readonly IProductService _productService;

    public ProductsController(IProductRepo productRepo, IProductToUserRepo productToUserRepo, IMapper mapper, IProductService productService)
    {
        _productRepo = productRepo;
        _mapper = mapper;
        _productToUserRepo = productToUserRepo;
        _productService = productService;
    }

    [HttpGet(Name = "GetProducts")]
    public ActionResult<IEnumerable<ProductReadDto>> GetProducts()
    {
        var products = _productRepo.GetAllProducts();
        return Ok(_mapper.Map<IEnumerable<ProductReadDto>>(products));
    }

    [HttpGet("{id}", Name = "GetProductById")]
    public ActionResult<ProductReadDto> GetProductById(int id)
    {
        var product = _productRepo.GetProductById(id);
        if (product is not null)
        {
            return Ok(_mapper.Map<ProductReadDto>(product));
        }

        return NotFound();
    }

    [HttpPost]
    public ActionResult<ProductReadDto> CreateProduct(ProductCreateDto productDto)
    {
        var product = _mapper.Map<Product>(productDto);
        _productRepo.CreateProduct(product);
        _productRepo.SaveChanges();

        var productReadDto = _mapper.Map<ProductReadDto>(product);

        return CreatedAtRoute(nameof(GetProductById), new { Id = product.Id }, productReadDto);
    }

    [HttpGet]
    [Route("GetProductsByUserId/{userId}")]
    public async Task<ActionResult<IEnumerable<ProductReadDto>>> GetProductsByUserId(int userId)
    {
        var products = await _productService.GetProductsByUserId(userId);
        return Ok(_mapper.Map<IEnumerable<ProductReadDto>>(products));
    }
}