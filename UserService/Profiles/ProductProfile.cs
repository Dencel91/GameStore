using AutoMapper;
using UserService.Dtos;

namespace UserService.Profiles;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<ProductService.GrpcProductModel, ProductDto>();
        CreateMap<UserService.GrpcProductModel, ProductDto>();
        CreateMap<ProductDto, UserService.GrpcProductModel>();
    }
}
