using Data.Interfaces.Base;
using Data.Models.DatabaseModels;

namespace Data.Interfaces
{
    public interface IPermissionTypeRepository : IBaseRepository<PermissionType>
    {
        Task<PermissionType?> GetPermissionTypeByDescriptionAsync(string description);
    }
}