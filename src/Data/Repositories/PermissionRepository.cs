using AutoMapper;
using Data.Interfaces;
using Data.Models.DatabaseModels;
using Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    public class PermissionRepository : BaseRepository<Permission>, IPermissionRepository
    {
        public PermissionRepository(RestApiN5DbContext db, IMapper mapper)
        : base(db, mapper)
        {
            EntityName = "Permission";
        }

        public async Task<Permission?> GetPermissionByDescriptionAsync(string description)
        {
            return await Table.Where(u => u.Description == description).FirstOrDefaultAsync();
        }

        public async Task<List<Permission>> GetPermissionByTypeIdAsync(long id)
        {
            return await Table
                .Where(u => u.PermissionTypeId == id)
                .ToListAsync();
        }

        public async Task DeleteRangeAsync(List<Permission> permissions)
        {
            if (permissions != null && permissions.Any())
            {
                Table.RemoveRange(permissions);
            }
        }
    }
}