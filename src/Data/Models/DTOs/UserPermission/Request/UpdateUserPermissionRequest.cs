namespace Data.Models.DTOs.UserPermission.Request
{
    public class UpdateUserPermissionRequest
    {
        public long UserId { get; set; }

        public long PermissionId { get; set; }
    }
}
