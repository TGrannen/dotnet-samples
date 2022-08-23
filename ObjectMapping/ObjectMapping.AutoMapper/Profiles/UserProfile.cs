using AutoMapper;
using ObjectMapping.AutoMapper.Models;

namespace ObjectMapping.AutoMapper.Profiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserViewModel>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.EmailAddress))
            .ForMember(dest => dest.IsAdult, opt => opt.Condition(src => src.Age > 18))
            .ReverseMap();
    }
}