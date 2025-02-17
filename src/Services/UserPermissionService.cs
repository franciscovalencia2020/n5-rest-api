using Services.Interfaces;
using Data.Interfaces;
using Data.Models.DTOs.UserPermission.Response;
using Data.Models.DTOs.UserPermission.Request;
using AutoMapper;
using Data.Models.DatabaseModels;
using Data.Helpers;

namespace Services
{
    public class UserPermissionService : IUserPermissionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserPermissionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
             _unitOfWork = unitOfWork;
             _mapper = mapper;
        }

        public async Task<UserPermissionResponse> CreateUserPermission(CreateUserPermissionRequest request)
        {
            if (request.UserId <= 0)
            {
                throw new ArgumentException("Invalid UserId");
            }

            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
            if (user is null)
            {
                throw new EntityNotFoundException("User not found with id: " + request.UserId);
            }

            if (request.PermissionId <= 0)
            {
                throw new ArgumentException("Invalid PermissionId");
            }

            var permission = await _unitOfWork.Permissions.GetByIdAsync(request.PermissionId);
            if (permission is null)
            {
                throw new EntityNotFoundException("Permission not found with id: " + request.PermissionId);
            }

            var existingPermission = await _unitOfWork.UserPermissions.GetUserPermissionAsync(request.UserId, request.PermissionId);
            if (existingPermission != null)
            {
                throw new ArgumentException("User already has this permission.");
            }

            var newUserPermission = _mapper.Map<UserPermission>(request);
            await _unitOfWork.UserPermissions.AddAsync(newUserPermission);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<UserPermissionResponse>(newUserPermission);
        }

        public async Task<List<UserPermissionResponse>> GetUserPermissions()
        {
            return await _unitOfWork.UserPermissions.AllAsync<UserPermissionResponse>();
        }

        public async Task<PaginatedList<UserPermissionResponse>> GetUserPermissionsPaginated(int pageNumber, int pageSize)
        {
            var userPermissions = _unitOfWork.UserPermissions.AllAsyncQueryable<UserPermission>();
            var paginatedUserPermissions = await PaginatedList<UserPermission>.CreateAsync(userPermissions.AsQueryable(), pageNumber, pageSize);
            var userPermissionResponses = _mapper.Map<List<UserPermissionResponse>>(paginatedUserPermissions.Items);

            return new PaginatedList<UserPermissionResponse>(
                userPermissionResponses,
                paginatedUserPermissions.TotalCount,
                pageNumber,
                pageSize
            );
        }

        public async Task<UserPermissionResponse> GetUserPermissionById(long id)
        {
            var userPermission = await _unitOfWork.UserPermissions.GetByIdAsync(id);
            if (userPermission is null)
            {
                throw new EntityNotFoundException("UserPermission not found with id: " + id);
            }
            return _mapper.Map<UserPermissionResponse>(userPermission);
        }

        public async Task<List<UserPermissionResponse>> GetUserPermissionByUserId(long id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user is null)
            {
                throw new EntityNotFoundException("User not found with id: " + id);
            }

            var userPermissions = await _unitOfWork.UserPermissions.GetUserPermissionByUserIdAsync(id);
            return _mapper.Map<List<UserPermissionResponse>>(userPermissions);
        }

        public async Task<UserPermissionResponse> UpdateUserPermission(long id, UpdateUserPermissionRequest request)
        {
            if (request.UserId <= 0)
            {
                throw new ArgumentException("Invalid UserId");
            }

            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
            if (user is null)
            {
                throw new EntityNotFoundException("User not found with id: " + request.UserId);
            }

            if (request.PermissionId <= 0)
            {
                throw new ArgumentException("Invalid PermissionId");
            }

            var permission = await _unitOfWork.Permissions.GetByIdAsync(request.PermissionId);
            if (permission is null)
            {
                throw new EntityNotFoundException("Permission not found with id: " + request.PermissionId);
            }

            var userPermission = await _unitOfWork.UserPermissions.GetByIdAsync(id);
            if (userPermission is null)
            {
                throw new EntityNotFoundException("UserPermission not found with id: " + id);
            }

            if ((userPermission.UserId != request.UserId || userPermission.PermissionId != request.PermissionId) && await _unitOfWork.UserPermissions.GetUserPermissionAsync(request.UserId, request.PermissionId) != null)
            {
                throw new ArgumentException("User already has this permission.");
            }

            _mapper.Map(request, userPermission);
            userPermission.UpdatedDate = DateTime.UtcNow;
            await _unitOfWork.UserPermissions.UpdateAsync(userPermission);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<UserPermissionResponse>(userPermission);
        }

        public async Task<bool> DeleteUserPermission(long id)
        {
            var userPermission = await _unitOfWork.UserPermissions.GetByIdAsync(id);
            if (userPermission is null)
            {
                return false;
            }
            await _unitOfWork.UserPermissions.DeleteAsync(id);
            await _unitOfWork.CompleteAsync();
            return true;
        }
    }
}
