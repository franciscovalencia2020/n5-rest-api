using AutoMapper;
using Data.Helpers;
using Data.Models.DTOs.Permission.Response;
using MediatR;
using Services.Commands.Permission;
using Services.ElasticSearch.Interfaces;
using Services.Kafka.interfaces;

namespace Services.Handlers.Permission
{
    public class UpdatePermissionHandler : IRequestHandler<UpdatePermissionCommand, PermissionResponse>
    {
        private readonly IPermissionElasticService _elasticService;
        private readonly IMapper _mapper;
        private readonly IKafkaProducerService _kafkaProducer;


        public UpdatePermissionHandler(IPermissionElasticService elasticService, IMapper mapper, IKafkaProducerService kafkaProducerService)
        {
            _elasticService = elasticService;
            _mapper = mapper;
            _kafkaProducer = kafkaProducerService;
        }

        public async Task<PermissionResponse> Handle(UpdatePermissionCommand request, CancellationToken cancellationToken)
        {
            var permission = await _elasticService.GetPermissionByIdAsync(request.Id);
            if (permission == null)
            {
                throw new EntityNotFoundException($"Permission not found with id: {request.Id}");
            }

            var permissionExists = await _elasticService.SearchPermissionsAsync(request.Description);
            if (permissionExists.Any(p => p.Id != request.Id))
            {
                throw new ArgumentException("There is already a Permission with that name.");
            }

            _mapper.Map(request, permission);
            permission.UpdatedDate = DateTime.UtcNow;

            await _elasticService.UpdatePermissionAsync(permission);
            await _kafkaProducer.ProduceAsync("modify", $"A permission was updated successfully: {Guid.NewGuid()}");
            return _mapper.Map<PermissionResponse>(permission);
        }
    }
}
