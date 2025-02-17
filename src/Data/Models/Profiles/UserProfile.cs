using AutoMapper;
using Data.Models.DatabaseModels;
using Data.Models.DTOs.User.Request;
using Data.Models.DTOs.User.Response;

namespace Data.Models.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, User>().ReverseMap();
            CreateMap<User, CreateUserRequest>().ReverseMap();
            CreateMap<User, UserResponse>().ReverseMap();
            CreateMap<User, UpdateUserRequest>().ReverseMap();
        }
    }
}
