using Data.Helpers;
using Data.Interfaces;
using MediatR;
using Services.Commands.Permission;
using Services.ElasticSearch.Interfaces;
using Services.Kafka.interfaces;

namespace Services.Handlers.Permission
{
    public class DeletePermissionHandler : IRequestHandler<DeletePermissionCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPermissionElasticService _elasticService;
        private readonly IKafkaProducerService _kafkaProducer;


        public DeletePermissionHandler(IUnitOfWork unitOfWork, IPermissionElasticService elasticService, IKafkaProducerService kafkaProducerService)
        {
            _unitOfWork = unitOfWork;
            _elasticService = elasticService;
            _kafkaProducer = kafkaProducerService;
        }

        public async Task<bool> Handle(DeletePermissionCommand request, CancellationToken cancellationToken)
        {
            var permission = await _unitOfWork.Permissions.GetByIdAsync(request.Id);
            if (permission is null)
            {
                throw new EntityNotFoundException($"Permission not found with id: {request.Id}");
            }

            await _unitOfWork.Permissions.DeleteAsync(request.Id);
            var userPermissions = await _unitOfWork.UserPermissions.GetUserPermissionByPermissionIdAsync(request.Id);
            if (userPermissions.Any())
            {
                await _unitOfWork.UserPermissions.DeleteRangeAsync(userPermissions);
            }
            await _unitOfWork.CompleteAsync();
            await _kafkaProducer.ProduceAsync("modify", $"A permission was removed successfully: {Guid.NewGuid()}");
            await _elasticService.DeletePermissionAsync(request.Id);

            return true;
        }
    }
}
