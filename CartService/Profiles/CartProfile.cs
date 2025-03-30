using AutoMapper;
using CartService.DTOs;
using CartService.Models;

namespace CartService.Profiles
{
    public class CartProfile : Profile
    {
        public CartProfile()
        {
            CreateMap<Cart, CartDto>()
                .ForMember(dest => dest.Products, opt => opt.Ignore());
                
        }
    }
}
