using AutoMapper;
using ProductService.DTOs;
using ProductService.Models;

namespace ProductService.Profiles;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<Product, ProductDto>();
        CreateMap<CreateProductRequest, Product>()
            .ForMember(dest => dest.Images, opt => opt.Ignore());
        CreateMap<Product, GrpcProductModel>();
        CreateMap<CreateProductReviewRequest, ProductReview>();
        CreateMap<Product, ProductDetailsResponse>()
            .ForMember(dist => dist.Images, opt => opt.MapFrom(product => product.Images.Where(i => i.Type == Models.enums.ImageType.Preview).Select(i => i.Url)));
    }
}
