using Data.Interfaces.Base;
using Data.Models.DatabaseModels;

namespace Data.Interfaces
{
    public interface IUserPermissionRepository : IBaseRepository<UserPermission>
    {
        Task<UserPermission?> GetUserPermissionAsync(long userId, long permissionId);
        Task<List<UserPermission>> GetUserPermissionByUserIdAsync(long userId);
        Task DeleteRangeAsync(List<UserPermission> userPermissions);
        Task<List<UserPermission>> GetUserPermissionByPermissionIdAsync(long id);
    }
}