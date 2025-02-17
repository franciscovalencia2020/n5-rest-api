using AutoMapper;
using Data.Models.DTOs.Permission.Response;
using MediatR;
using Services.ElasticSearch.Interfaces;
using Services.Kafka.interfaces;
using Services.Queries.Permission;

namespace Services.Handlers.Permission
{
    public class GetPermissionsHandler : IRequestHandler<GetPermissionsQuery, List<PermissionResponse>>
    {
        private readonly IPermissionElasticService _elasticService;
        private readonly IMapper _mapper;
        private readonly IKafkaProducerService _kafkaProducer;

        public GetPermissionsHandler(IPermissionElasticService elasticService, IMapper mapper, IKafkaProducerService kafkaProducerService)
        {
            _elasticService = elasticService;
            _mapper = mapper;
            _kafkaProducer = kafkaProducerService;
        }

        public async Task<List<PermissionResponse>> Handle(GetPermissionsQuery request, CancellationToken cancellationToken)
        {
            var permissions = await _elasticService.GetAllPermissionsAsync();
            await _kafkaProducer.ProduceAsync("get", $"All permissions were retrieved successfully: {Guid.NewGuid()}");
            return _mapper.Map<List<PermissionResponse>>(permissions);
        }
    }
}
