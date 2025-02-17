using AutoMapper;
using Data.Interfaces;
using Data.Models.DatabaseModels;
using Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(RestApiN5DbContext db, IMapper mapper)
        : base(db, mapper)
        {
            EntityName = "User";
        }
        public async Task<User?> GetUserByEmailAsync(string email, bool isLogin = false)
        {
            var query = Table.Where(u => u.Email == email);

            if (!isLogin)
            {
                query = query.IgnoreQueryFilters();
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task<User?> GetUserByRefreshTokenAsync(string refreshToken)
        {
            return await Table.Where(u => u.RefreshToken == refreshToken).IgnoreQueryFilters().FirstOrDefaultAsync();
        }
    }
}
