using Data.Models.DTOs.PermissionType.Response;

namespace Data.Models.DTOs.Permission.Response
{
    public class PermissionResponse : BaseEntityDto
    {
        public long PermissionTypeId { get; set; }
        public string Description { get; set; }

        public PermissionTypeResponse PermissionType { get; set; }
    }
}
