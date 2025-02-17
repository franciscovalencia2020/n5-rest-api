using AutoMapper;
using AutoMapper.QueryableExtensions;
using Data.Interfaces.Base;
using Data.Models.DatabaseModels;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories.Base
{
    public class BaseRepository<Entity> : IBaseRepository<Entity> where Entity : BaseEntity
    {
        protected readonly IMapper Mapper;
        protected readonly RestApiN5DbContext Db;
        protected DbSet<Entity> Table => Db.Set<Entity>();

        public string EntityName { get; protected set; }

        public BaseRepository(RestApiN5DbContext db, IMapper mapper)
        {
            Db = db;
            Mapper = mapper;
        }

        public virtual async Task<long> AddAsync(Entity entity)
        {
            entity.CreatedDate = DateTime.UtcNow;
            entity.UpdatedDate = entity.CreatedDate;
            Db.Add(entity);
            return entity.Id;
        }

        public virtual async Task<Entity?> GetByIdAsync(long id) => await Table.FindAsync(id);

        public virtual async Task UpdateAsync(Entity entity)
        {
            entity.UpdatedDate = DateTime.UtcNow;
            Table.Update(entity);
        }

        public virtual async Task<bool> DeleteAsync(long id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null) return false;

            entity.IsDeleted = true;
            await UpdateAsync(entity);
            return true;
        }

        public virtual async Task<List<Dto>> AllAsync<Dto>()
        {
            return await Table.ProjectTo<Dto>(Mapper.ConfigurationProvider).ToListAsync();
        }

        public virtual IQueryable<Dto> AllAsyncQueryable<Dto>() where Dto : class
        {
            return Table.ProjectTo<Dto>(Mapper.ConfigurationProvider);
        }
    }
}
