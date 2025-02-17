using AutoMapper;
using Data.Models.DatabaseModels;
using Data.Models.DTOs.PermissionType.Request;
using Data.Models.DTOs.PermissionType.Response;

namespace Data.Models.Profiles
{
    public class PermissionTypeProfile : Profile
    {
        public PermissionTypeProfile()
        {
            CreateMap<PermissionType, PermissionType>().ReverseMap();
            CreateMap<PermissionType, CreatePermissionTypeRequest>().ReverseMap();
            CreateMap<PermissionType, PermissionTypeResponse>().ReverseMap();
            CreateMap<PermissionType, UpdatePermissionTypeRequest>().ReverseMap();
        }
    }
}