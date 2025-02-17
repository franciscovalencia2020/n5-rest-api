using AutoMapper;
using Data.Interfaces;
using Data.Models.DatabaseModels;
using Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    public class UserPermissionRepository : BaseRepository<UserPermission>, IUserPermissionRepository
    {
        public UserPermissionRepository(RestApiN5DbContext db, IMapper mapper)
        : base(db, mapper)
        {
            EntityName = "UserPermission";
        }

        public async Task<UserPermission?> GetUserPermissionAsync(long userId, long permissionId)
        {
            return await Table.Where(u => u.UserId == userId && u.PermissionId == permissionId).IgnoreQueryFilters().FirstOrDefaultAsync();
        }

        public async Task<List<UserPermission>> GetUserPermissionByUserIdAsync(long userId)
        {
            return await Table
                .Where(u => u.UserId == userId)
                .Include(up => up.Permission)
                .ToListAsync();
        }

        public async Task DeleteRangeAsync(List<UserPermission> userPermissions)
        {
            if (userPermissions != null && userPermissions.Any())
            {
                Table.RemoveRange(userPermissions);
            }
        }

        public async Task<List<UserPermission>> GetUserPermissionByPermissionIdAsync(long id)
        {
            return await Table
                .Where(u => u.PermissionId == id)
                .ToListAsync();
        }
    }
}