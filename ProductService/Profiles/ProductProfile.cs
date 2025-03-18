using AutoMapper;
using ProductService.DTOs;
using ProductService.Models;

namespace ProductService.Profiles;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<Product, ProductReadDto>();
        CreateMap<ProductCreateDto, Product>();
        CreateMap<Product, GrpcProductModel>();
    }
}
