using Services.Interfaces;
using Data.Interfaces;
using Data.Models.DTOs.User.Response;
using Data.Models.DTOs.User.Request;
using AutoMapper;
using Data.Models.DatabaseModels;
using Services.Helpers;
using Data.Helpers;
using Microsoft.Extensions.Configuration;
using Data.Extensions;

namespace Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly string _jwtSecretKey;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _jwtSecretKey = configuration.GetJwtSecretKey();
        }

        public async Task<UserResponse> CreateUser(CreateUserRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Password) || string.IsNullOrWhiteSpace(request.Email))
            {
                throw new ArgumentException("Email and Password are required.");
            }

            var userExist = await _unitOfWork.Users.GetUserByEmailAsync(request.Email);
            if (userExist is not null)
            {
                throw new InvalidOperationException($"A user with email {request.Email} already exists.");
            }

            var newUser = _mapper.Map<User>(request);

            newUser.PasswordHash = PasswordHelper.HashPassword(request.Password.Trim());

            await _unitOfWork.Users.AddAsync(newUser);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<UserResponse>(newUser);
        }

        public async Task<List<UserResponse>> GetUsers()
        {
            return await _unitOfWork.Users.AllAsync<UserResponse>();
        }

        public async Task<PaginatedList<UserResponse>> GetUsersPaginated(int pageNumber, int pageSize)
        {
            var users = _unitOfWork.Users.AllAsyncQueryable<User>();
            var paginatedUsers = await PaginatedList<User>.CreateAsync(users.AsQueryable(), pageNumber, pageSize);

            var userResponses = _mapper.Map<List<UserResponse>>(paginatedUsers.Items);

            return new PaginatedList<UserResponse>(
                userResponses,
                paginatedUsers.TotalCount,
                pageNumber,
                pageSize
            );
        }

        public async Task<UserResponse> GetUserById(long id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user is null)
            {
                throw new EntityNotFoundException("User not found with id: " + id);
            }
            return _mapper.Map<UserResponse>(user);
        }

        public async Task<UserResponse> UpdateUser(long id, UpdateUserRequest request)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user is null)
            {
                throw new EntityNotFoundException("User not found with id: " + id);
            }

            if (user.Email != request.Email)
            {
                var userExist = await _unitOfWork.Users.GetUserByEmailAsync(request.Email);
                if (userExist is not null)
                {
                    throw new InvalidOperationException($"A user with email {request.Email} already exists.");
                }
            }

            _mapper.Map(request, user);
            user.UpdatedDate = DateTime.UtcNow;

            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<UserResponse>(user);
        }

        public async Task<bool> DeleteUser(long id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user is null)
            {
                return false;
            }

            var userPermissions = await _unitOfWork.UserPermissions.GetUserPermissionByUserIdAsync(id);
            if (userPermissions.Any())
            {
                await _unitOfWork.UserPermissions.DeleteRangeAsync(userPermissions);
            }

            await _unitOfWork.Users.DeleteAsync(id);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<AuthResponseDto> LoginAsync(string email, string password)
        {
            var user = await _unitOfWork.Users.GetUserByEmailAsync(email, true);
            if (user == null || !PasswordHelper.VerifyPassword(password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid credentials.");
            }

            var token = JwtHelper.GenerateToken(user, _jwtSecretKey);
            var refreshToken = JwtHelper.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.TokenExpiration = DateTime.UtcNow.AddDays(7);
            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.CompleteAsync();
            return new AuthResponseDto
            {
                User = _mapper.Map<UserResponse>(user),
                Token = token,
                RefreshToken = refreshToken
            };
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                throw new ArgumentException("RefreshToken is required.");
            }

            var user = await _unitOfWork.Users.GetUserByRefreshTokenAsync(request.RefreshToken);
            if (user == null || user.TokenExpiration <= DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Invalid or expired refresh token.");
            }

            var newToken = JwtHelper.GenerateToken(user, _jwtSecretKey);
            var newRefreshToken = JwtHelper.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.TokenExpiration = DateTime.UtcNow.AddDays(7);
            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.CompleteAsync();
            return new AuthResponseDto
            {
                Token = newToken,
                RefreshToken = newRefreshToken
            };
        }
    }
}
