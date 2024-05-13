using AutoMapper;
using hmgAPI.Entities;
using hmgAPI.DTOs;

namespace API.Helpers;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<RegisterDto, AppUser>();
        CreateMap<RegisterDto, AppMerchant>();
        CreateMap<LoginDto, AppUser>();
        //
        CreateMap<UserDto, AppUser>();
        //
        CreateMap<UserDto, AppMerchant>();

    }
}