using AutoMapper;
using Data.Models.DatabaseModels;
using Data.Models.DTOs.Permission.Request;
using Data.Models.DTOs.Permission.Response;

namespace Data.Models.Profiles
{
    public class PermissionProfile : Profile
    {
        public PermissionProfile()
        {
            CreateMap<Permission, Permission>().ReverseMap();
            CreateMap<Permission, CreatePermissionRequest>().ReverseMap();
            CreateMap<Permission, PermissionResponse>().ReverseMap();
            CreateMap<Permission, UpdatePermissionRequest>().ReverseMap();
        }
    }
}
