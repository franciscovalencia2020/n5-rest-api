using AutoMapper;
using Data.Models.DatabaseModels;
using Services.Commands.Permission;

namespace Services.Profiles
{
    public class PermissionProfile : Profile
    {
        public PermissionProfile()
        {
            CreateMap<CreatePermissionCommand, Permission>().ReverseMap();
            CreateMap<UpdatePermissionCommand, Permission>().ReverseMap();
        }
    }
}