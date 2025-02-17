using AutoMapper;
using Data.Helpers;
using Data.Models.DTOs.Permission.Response;
using MediatR;
using Services.ElasticSearch.Interfaces;
using Services.Kafka.interfaces;
using Services.Queries.Permission;

namespace Services.Handlers.Permission
{
    public class GetPermissionByIdHandler : IRequestHandler<GetPermissionByIdQuery, PermissionResponse>
    {
        private readonly IPermissionElasticService _elasticService;
        private readonly IMapper _mapper;
        private readonly IKafkaProducerService _kafkaProducer;

        public GetPermissionByIdHandler(IPermissionElasticService elasticService, IMapper mapper, IKafkaProducerService kafkaProducerService)
        {
            _elasticService = elasticService;
            _mapper = mapper;
            _kafkaProducer = kafkaProducerService;
        }

        public async Task<PermissionResponse> Handle(GetPermissionByIdQuery request, CancellationToken cancellationToken)
        {
            var permission = await _elasticService.GetPermissionByIdAsync(request.Id);
            if (permission == null)
            {
                throw new EntityNotFoundException($"Permission not found with id: {request.Id}");
            }
            await _kafkaProducer.ProduceAsync("get", $"A permission was retrieved successfully: {Guid.NewGuid()}");
            return _mapper.Map<PermissionResponse>(permission);
        }
    }
}
