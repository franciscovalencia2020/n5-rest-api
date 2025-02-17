using Data.Helpers;
using Data.Models.DTOs.UserPermission.Request;
using Data.Models.DTOs.UserPermission.Response;

namespace Services.Interfaces
{
    public interface IUserPermissionService
    {
        Task<UserPermissionResponse> CreateUserPermission(CreateUserPermissionRequest request);
        Task<List<UserPermissionResponse>> GetUserPermissions();
        Task<PaginatedList<UserPermissionResponse>> GetUserPermissionsPaginated(int pageNumber, int pageSize);
        Task<UserPermissionResponse> GetUserPermissionById(long id);
        Task<List<UserPermissionResponse>> GetUserPermissionByUserId(long id);
        Task<UserPermissionResponse> UpdateUserPermission(long id, UpdateUserPermissionRequest request);
        Task<bool> DeleteUserPermission(long id);
    }
}
