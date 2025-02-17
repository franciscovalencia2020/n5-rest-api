using Data.Interfaces.Base;
using Data.Models.DatabaseModels;

namespace Data.Interfaces
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User?> GetUserByEmailAsync(string email, bool isLogin = false);
        Task<User?> GetUserByRefreshTokenAsync(string refreshToken);
    }
}
