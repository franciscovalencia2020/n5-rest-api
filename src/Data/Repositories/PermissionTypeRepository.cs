using AutoMapper;
using Data.Interfaces;
using Data.Models.DatabaseModels;
using Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    public class PermissionTypeRepository : BaseRepository<PermissionType>, IPermissionTypeRepository
    {
        public PermissionTypeRepository(RestApiN5DbContext db, IMapper mapper)
        : base(db, mapper)
        {
            EntityName = "PermissionType";
        }

        public async Task<PermissionType?> GetPermissionTypeByDescriptionAsync(string description)
        {
            return await Table.Where(u => u.Description == description).FirstOrDefaultAsync();
        }
    }
}