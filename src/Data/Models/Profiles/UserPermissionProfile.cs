using AutoMapper;
using Data.Models.DatabaseModels;
using Data.Models.DTOs.UserPermission.Request;
using Data.Models.DTOs.UserPermission.Response;

namespace Data.Models.Profiles
{
    public class UserPermissionProfile : Profile
    {
        public UserPermissionProfile()
        {
            CreateMap<UserPermission, UserPermission>().ReverseMap();
            CreateMap<UserPermission, CreateUserPermissionRequest>().ReverseMap();
            CreateMap<UserPermission, UserPermissionResponse>().ReverseMap();
            CreateMap<UserPermission, UpdateUserPermissionRequest>().ReverseMap();
        }
    }
}