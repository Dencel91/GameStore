using AutoMapper;
using CartService.DTOs;
using ProductService;

namespace CartService.Profiles;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<GrpcProductModel, ProductDto>();
    }
}
