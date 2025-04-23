using AutoMapper;
using UserService.Dtos;
using UserService.Models;

namespace UserService.Profiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<AddUserRequest, User>();
    }
}