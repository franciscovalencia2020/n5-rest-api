using Data.Models.DTOs.Permission.Response;
using Data.Models.DTOs.User.Response;

namespace Data.Models.DTOs.UserPermission.Response
{
    public class UserPermissionResponse : BaseEntityDto
    {
        public long UserId { get; set; }

        public long PermissionId { get; set; }

        public UserResponse User { get; set; }

        public PermissionResponse Permission { get; set; }
    }
}
