using AutoMapper;
using Data.Helpers;
using Data.Interfaces;
using Data.Models.DatabaseModels;
using Data.Models.DTOs.Permission.Response;
using Moq;
using Services.Commands.Permission;
using Services.ElasticSearch.Interfaces;
using Services.Handlers.Permission;
using Services.Kafka.interfaces;

namespace Api.Tests
{
    public class UpdatePermissionHandlerIntegrationTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IPermissionElasticService> _elasticServiceMock;
        private readonly Mock<IKafkaProducerService> _kafkaProducerMock;
        private readonly UpdatePermissionHandler _updatePermissionHandler;

        public UpdatePermissionHandlerIntegrationTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _elasticServiceMock = new Mock<IPermissionElasticService>();
            _kafkaProducerMock = new Mock<IKafkaProducerService>();

            _updatePermissionHandler = new UpdatePermissionHandler(
                _elasticServiceMock.Object,
                _mapperMock.Object,
                _kafkaProducerMock.Object
            );
        }

        [Fact]
        public async Task Handle_UpdatePermission_ReturnsUpdatedPermissionResponse()
        {
            var request = new UpdatePermissionCommand(1, "Updated Permission", 1);

            var existingPermission = new Permission
            {
                Id = 1,
                Description = "Original Permission",
                PermissionTypeId = 1
            };

            var updatedPermissionResponse = new PermissionResponse
            {
                Id = 1,
                Description = "Updated Permission",
                PermissionTypeId = 1
            };

            _elasticServiceMock.Setup(e => e.GetPermissionByIdAsync(request.Id))
                .ReturnsAsync(existingPermission);

            _elasticServiceMock.Setup(e => e.SearchPermissionsAsync(request.Description))
                .ReturnsAsync(new List<Permission>());

            _mapperMock.Setup(m => m.Map(request, existingPermission))
                .Callback<UpdatePermissionCommand, Permission>((req, perm) =>
                {
                    perm.Description = req.Description;
                    perm.PermissionTypeId = req.PermissionTypeId;
                    perm.UpdatedDate = DateTime.UtcNow;
                });

            _elasticServiceMock.Setup(e => e.UpdatePermissionAsync(existingPermission))
                .Returns(Task.CompletedTask);

            _kafkaProducerMock.Setup(k => k.ProduceAsync("modify", It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            _mapperMock.Setup(m => m.Map<PermissionResponse>(existingPermission))
                .Returns(updatedPermissionResponse);

            var result = await _updatePermissionHandler.Handle(request, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal("Updated Permission", result.Description);
            Assert.Equal(1, result.PermissionTypeId);

            _elasticServiceMock.Verify(e => e.GetPermissionByIdAsync(request.Id), Times.Once);
            _elasticServiceMock.Verify(e => e.SearchPermissionsAsync(request.Description), Times.Once);
            _elasticServiceMock.Verify(e => e.UpdatePermissionAsync(existingPermission), Times.Once);
            _kafkaProducerMock.Verify(k => k.ProduceAsync("modify", It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task Handle_UpdatePermission_ThrowsEntityNotFoundException()
        {
            var request = new UpdatePermissionCommand(1, "Updated Permission", 1);

            _elasticServiceMock.Setup(e => e.GetPermissionByIdAsync(request.Id))
                .ReturnsAsync((Permission)null);

            await Assert.ThrowsAsync<EntityNotFoundException>(() => _updatePermissionHandler.Handle(request, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_UpdatePermission_ThrowsArgumentExceptionForDuplicateDescription()
        {
            var request = new UpdatePermissionCommand(1, "Updated Permission", 1);

            var existingPermission = new Permission
            {
                Id = 1,
                Description = "Original Permission",
                PermissionTypeId = 1
            };

            var duplicatePermission = new Permission
            {
                Id = 2,
                Description = "Updated Permission",
                PermissionTypeId = 1
            };

            _elasticServiceMock.Setup(e => e.GetPermissionByIdAsync(request.Id))
                .ReturnsAsync(existingPermission);

            _elasticServiceMock.Setup(e => e.SearchPermissionsAsync(request.Description))
                .ReturnsAsync(new List<Permission> { duplicatePermission });

            await Assert.ThrowsAsync<ArgumentException>(() => _updatePermissionHandler.Handle(request, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_UpdatePermission_SendsKafkaMessage()
        {
            var request = new UpdatePermissionCommand(1, "Updated Permission", 1);

            var existingPermission = new Permission
            {
                Id = 1,
                Description = "Original Permission",
                PermissionTypeId = 1
            };

            _elasticServiceMock.Setup(e => e.GetPermissionByIdAsync(request.Id))
                .ReturnsAsync(existingPermission);

            _elasticServiceMock.Setup(e => e.SearchPermissionsAsync(request.Description))
                .ReturnsAsync(new List<Permission>());

            _elasticServiceMock.Setup(e => e.UpdatePermissionAsync(existingPermission))
                .Returns(Task.CompletedTask);

            _kafkaProducerMock.Setup(k => k.ProduceAsync("modify", It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            await _updatePermissionHandler.Handle(request, CancellationToken.None);

            _kafkaProducerMock.Verify(k => k.ProduceAsync("modify", It.IsAny<string>()), Times.Once);
        }
    }
}