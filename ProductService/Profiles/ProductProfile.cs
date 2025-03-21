using AutoMapper;
using ProductService.DTOs;
using ProductService.Models;

namespace ProductService.Profiles;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<Product, ProductResponse>();
        CreateMap<CreateProductRequest, Product>();
        CreateMap<Product, GrpcProductModel>();
        CreateMap<CreateProductReviewRequest, ProductReview>();
        CreateMap<Product, ProductDetailsResponse>()
            .ForMember(dist => dist.Images, opt => opt.MapFrom(product => product.Images.Select(i => i.ImageUrl)));
    }
}
