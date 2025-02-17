namespace Data.Models.DTOs.Permission.Request
{
    public class CreatePermissionRequest
    {
        public long PermissionTypeId { get; set; }
        public string Description { get; set; }
    }
}
