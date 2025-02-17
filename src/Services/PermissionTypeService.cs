using AutoMapper;
using Data.Helpers;
using Data.Interfaces;
using Data.Models.DatabaseModels;
using Data.Models.DTOs.PermissionType.Request;
using Data.Models.DTOs.PermissionType.Response;
using Services.ElasticSearch.Interfaces;
using Services.Interfaces;

namespace Services
{
    public class PermissionTypeService : IPermissionTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPermissionElasticService _elasticService;


        public PermissionTypeService(IUnitOfWork unitOfWork, IMapper mapper, IPermissionElasticService permissionElasticService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _elasticService = permissionElasticService;
        }

        public async Task<PermissionTypeResponse> CreatePermissionType(CreatePermissionTypeRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Description))
            {
                throw new ArgumentException("Description is required.");
            }

            var permissionTypeExists = await _unitOfWork.PermissionsType.GetPermissionTypeByDescriptionAsync(request.Description);
            if (permissionTypeExists is not null)
            {
                throw new ArgumentException("There is already a PermissionType with that name.");
            }

            var newPermissionType = _mapper.Map<PermissionType>(request);

            await _unitOfWork.PermissionsType.AddAsync(newPermissionType);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<PermissionTypeResponse>(newPermissionType);
        }

        public async Task<List<PermissionTypeResponse>> GetPermissionsType()
        {
            return await _unitOfWork.PermissionsType.AllAsync<PermissionTypeResponse>();
        }

        public async Task<PaginatedList<PermissionTypeResponse>> GetPermissionsTypePaginated(int pageNumber, int pageSize)
        {
            var permissionsType = _unitOfWork.PermissionsType.AllAsyncQueryable<PermissionType>();
            var paginatedPermissionsType = await PaginatedList<PermissionType>.CreateAsync(permissionsType.AsQueryable(), pageNumber, pageSize);

            var permissionsTypeResponses = _mapper.Map<List<PermissionTypeResponse>>(paginatedPermissionsType.Items);

            return new PaginatedList<PermissionTypeResponse>(
                permissionsTypeResponses,
                paginatedPermissionsType.TotalCount,
                pageNumber,
                pageSize
            );
        }

        public async Task<PermissionTypeResponse> GetPermissionTypeById(long id)
        {
            var permissionType = await _unitOfWork.PermissionsType.GetByIdAsync(id);
            if (permissionType is null)
            {
                throw new EntityNotFoundException("PermissionType not found with id: " + id);
            }
            return _mapper.Map<PermissionTypeResponse>(permissionType);
        }

        public async Task<PermissionTypeResponse> UpdatePermissionType(long id, UpdatePermissionTypeRequest request)
        {
            var permissionType = await _unitOfWork.PermissionsType.GetByIdAsync(id);
            if (permissionType is null)
            {
                throw new EntityNotFoundException("PermissionType not found with id: " + id);
            }

            var permissionTypeExists = await _unitOfWork.PermissionsType.GetPermissionTypeByDescriptionAsync(request.Description);
            if (permissionTypeExists is not null)
            {
                throw new ArgumentException("There is already a PermissionType with that name.");
            }

            _mapper.Map(request, permissionType);
            permissionType.UpdatedDate = DateTime.UtcNow;

            await _unitOfWork.PermissionsType.UpdateAsync(permissionType);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<PermissionTypeResponse>(permissionType);
        }

        public async Task<bool> DeletePermissionType(long id)
        {
            var permissionType = await _unitOfWork.PermissionsType.GetByIdAsync(id);
            if (permissionType is null)
            {
                return false;
            }

            var permissions = await _unitOfWork.Permissions.GetPermissionByTypeIdAsync(id);
            if (permissions.Any())
            {
                await _unitOfWork.Permissions.DeleteRangeAsync(permissions);
                await _elasticService.DeletePermissionsAsync(permissions);
            }

            await _unitOfWork.PermissionsType.DeleteAsync(id);
            await _unitOfWork.CompleteAsync();
            return true;
        }
    }
}
