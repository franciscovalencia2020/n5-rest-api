using AutoMapper;
using Data.Interfaces;
using Data.Models.DTOs.Permission.Response;
using MediatR;
using Services.Commands.Permission;
using PermissionModel = Data.Models.DatabaseModels.Permission;
using Data.Helpers;
using Services.ElasticSearch.Interfaces;
using Services.Kafka.interfaces;

namespace Services.Handlers.Permission
{
    public class CreatePermissionHandler : IRequestHandler<CreatePermissionCommand, PermissionResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPermissionElasticService _elasticService;
        private readonly IKafkaProducerService _kafkaProducer;

        public CreatePermissionHandler(IUnitOfWork unitOfWork, IMapper mapper, IPermissionElasticService elasticService, IKafkaProducerService kafkaProducer)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _elasticService = elasticService;
            _kafkaProducer = kafkaProducer;
        }

        public async Task<PermissionResponse> Handle(CreatePermissionCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.Description))
                throw new ArgumentException("Description is required.");

            var permissionExists = await _unitOfWork.Permissions.GetPermissionByDescriptionAsync(request.Description);
            if (permissionExists is not null)
                throw new ArgumentException("There is already a Permission with that name.");

            if (request.PermissionTypeId <= 0)
                throw new ArgumentException("Invalid PermissionTypeId");

            var permissionType = await _unitOfWork.PermissionsType.GetByIdAsync(request.PermissionTypeId);
            if (permissionType is null)
                throw new EntityNotFoundException("PermissionType not found with id: " + request.PermissionTypeId);

            var newPermission = _mapper.Map<PermissionModel>(request);
            await _unitOfWork.Permissions.AddAsync(newPermission);
            await _unitOfWork.CompleteAsync();

            await _elasticService.IndexPermissionAsync(newPermission);
            await _kafkaProducer.ProduceAsync("request", $"New permission created: {Guid.NewGuid()}");
            return _mapper.Map<PermissionResponse>(newPermission);
        }
    }
}
