using Data.Helpers;
using Data.Models.DTOs.PermissionType.Request;
using Data.Models.DTOs.PermissionType.Response;

namespace Services.Interfaces
{
    public interface IPermissionTypeService
    {
        Task<PermissionTypeResponse> CreatePermissionType(CreatePermissionTypeRequest request);
        Task<List<PermissionTypeResponse>> GetPermissionsType();
        Task<PaginatedList<PermissionTypeResponse>> GetPermissionsTypePaginated(int pageNumber, int pageSize);
        Task<PermissionTypeResponse> GetPermissionTypeById(long id);
        Task<PermissionTypeResponse> UpdatePermissionType(long id, UpdatePermissionTypeRequest request);
        Task<bool> DeletePermissionType(long id);
    }
}
