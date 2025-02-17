using Data.Helpers;
using Data.Models.DTOs.User.Request;
using Data.Models.DTOs.User.Response;

namespace Services.Interfaces
{
    public interface IUserService
    {
        Task<UserResponse> CreateUser(CreateUserRequest request);
        Task<List<UserResponse>> GetUsers();
        Task<PaginatedList<UserResponse>> GetUsersPaginated(int pageNumber, int pageSize);
        Task<UserResponse> GetUserById(long id);
        Task<UserResponse> UpdateUser(long id, UpdateUserRequest request);
        Task<bool> DeleteUser(long id);
        Task<AuthResponseDto> LoginAsync(string email, string password);
        Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequest request);
    }
}
