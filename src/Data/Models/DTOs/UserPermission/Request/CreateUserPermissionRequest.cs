namespace Data.Models.DTOs.UserPermission.Request
{
    public class CreateUserPermissionRequest
    {
        public long UserId { get; set; }

        public long PermissionId { get; set; }
    }
}
