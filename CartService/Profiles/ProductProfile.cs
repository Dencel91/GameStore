﻿using AutoMapper;
using CartService.DTOs;

namespace CartService.Profiles;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<ProductService.GrpcProductModel, ProductDto>();
        CreateMap<UserService.GrpcProductModel, ProductDto>();
    }
}
