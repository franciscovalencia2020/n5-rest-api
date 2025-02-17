using Data.Models.DatabaseModels;

namespace Data.Interfaces.Base
{
    public interface IBaseRepository<Entity> where Entity : BaseEntity
    {
        Task<long> AddAsync(Entity entity);
        Task<Entity?> GetByIdAsync(long id);
        Task UpdateAsync(Entity entity);
        Task<bool> DeleteAsync(long id);
        Task<List<Dto>> AllAsync<Dto>();
        IQueryable<Dto> AllAsyncQueryable<Dto>() where Dto : class;
    }
}
