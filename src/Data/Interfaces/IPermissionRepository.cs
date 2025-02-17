using Data.Interfaces.Base;
using Data.Models.DatabaseModels;

namespace Data.Interfaces
{
    public interface IPermissionRepository : IBaseRepository<Permission>
    {
        Task<Permission?> GetPermissionByDescriptionAsync(string description);
        Task<List<Permission>> GetPermissionByTypeIdAsync(long id);
        Task DeleteRangeAsync(List<Permission> permissions);
    }
}
