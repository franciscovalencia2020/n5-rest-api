using AutoMapper;
using Data.Helpers;
using Data.Models.DTOs.Permission.Response;
using MediatR;
using Services.ElasticSearch.Interfaces;
using Services.Kafka.interfaces;
using Services.Queries.Permission;

namespace Services.Handlers.Permission
{
    public class GetPermissionsPaginatedHandler : IRequestHandler<GetPermissionsPaginatedQuery, PaginatedList<PermissionResponse>>
    {
        private readonly IPermissionElasticService _elasticService;
        private readonly IMapper _mapper;
        private readonly IKafkaProducerService _kafkaProducer;


        public GetPermissionsPaginatedHandler(IPermissionElasticService elasticService, IMapper mapper, IKafkaProducerService kafkaProducerService)
        {
            _elasticService = elasticService;
            _mapper = mapper;
            _kafkaProducer = kafkaProducerService;
        }

        public async Task<PaginatedList<PermissionResponse>> Handle(GetPermissionsPaginatedQuery request, CancellationToken cancellationToken)
        {
            var elasticPermissions = await _elasticService.GetPaginatedPermissionsAsync(request.PageNumber, request.PageSize);
            var permissionResponses = _mapper.Map<List<PermissionResponse>>(elasticPermissions.Items);
            await _kafkaProducer.ProduceAsync("get", $"All permissions were retrieved successfully: {Guid.NewGuid()}");

            return new PaginatedList<PermissionResponse>(
                permissionResponses,
                elasticPermissions.TotalCount,
                request.PageNumber,
                request.PageSize
            );
        }
    }
}
