using Data.Helpers;
using Data.Models.DatabaseModels;

namespace Services.ElasticSearch.Interfaces
{
    public interface IPermissionElasticService
    {
        Task IndexPermissionAsync(Permission permission);
        Task<Permission> GetPermissionByIdAsync(long id);
        Task<IEnumerable<Permission>> GetAllPermissionsAsync();
        Task UpdatePermissionAsync(Permission permission);
        Task DeletePermissionAsync(long id);
        Task<PaginatedList<Permission>> GetPaginatedPermissionsAsync(int pageNumber, int pageSize);
        Task<IEnumerable<Permission>> SearchPermissionsAsync(string query);
        Task DeletePermissionsAsync(IEnumerable<Permission> permissions);
    }
}
