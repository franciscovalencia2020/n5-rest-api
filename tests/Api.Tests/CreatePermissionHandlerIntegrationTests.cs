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
    public class CreatePermissionHandlerIntegrationTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IPermissionElasticService> _elasticServiceMock;
        private readonly Mock<IKafkaProducerService> _kafkaProducerMock;
        private readonly CreatePermissionHandler _createPermissionHandler;

        public CreatePermissionHandlerIntegrationTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _elasticServiceMock = new Mock<IPermissionElasticService>();
            _kafkaProducerMock = new Mock<IKafkaProducerService>();

            _createPermissionHandler = new CreatePermissionHandler(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                _elasticServiceMock.Object,
                _kafkaProducerMock.Object
            );
        }

        [Fact]
        public async Task Handle_ValidRequest_ReturnsPermissionResponse()
        {
            var request = new CreatePermissionCommand("Test Permission", 1);
            var permissionType = new PermissionType { Id = 1, Description = "Test Type" };
            var newPermission = new Permission { Id = 1, Description = "Test Permission", PermissionTypeId = 1 };
            var permissionResponse = new PermissionResponse { Id = 1, Description = "Test Permission", PermissionTypeId = 1 };

            _unitOfWorkMock.Setup(u => u.Permissions.GetPermissionByDescriptionAsync(request.Description))
                .ReturnsAsync((Permission)null);

            _unitOfWorkMock.Setup(u => u.PermissionsType.GetByIdAsync(request.PermissionTypeId))
                .ReturnsAsync(permissionType);

            _unitOfWorkMock.Setup(u => u.Permissions.AddAsync(It.IsAny<Permission>()))
                .ReturnsAsync(1L);

            _unitOfWorkMock.Setup(u => u.CompleteAsync())
                .ReturnsAsync(1);

            _mapperMock.Setup(m => m.Map<Permission>(request))
                .Returns(newPermission);

            _mapperMock.Setup(m => m.Map<PermissionResponse>(newPermission))
                .Returns(permissionResponse);

            _elasticServiceMock.Setup(e => e.IndexPermissionAsync(newPermission))
                .Returns(Task.CompletedTask);

            _kafkaProducerMock.Setup(k => k.ProduceAsync("request", It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var result = await _createPermissionHandler.Handle(request, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(permissionResponse.Id, result.Id);
            Assert.Equal(permissionResponse.Description, result.Description);
            Assert.Equal(permissionResponse.PermissionTypeId, result.PermissionTypeId);

            _unitOfWorkMock.Verify(u => u.Permissions.GetPermissionByDescriptionAsync(request.Description), Times.Once);
            _unitOfWorkMock.Verify(u => u.PermissionsType.GetByIdAsync(request.PermissionTypeId), Times.Once);
            _unitOfWorkMock.Verify(u => u.Permissions.AddAsync(It.IsAny<Permission>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CompleteAsync(), Times.Once);
            _elasticServiceMock.Verify(e => e.IndexPermissionAsync(newPermission), Times.Once);
            _kafkaProducerMock.Verify(k => k.ProduceAsync("request", It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task Handle_InvalidDescription_ThrowsArgumentException()
        {
            var request = new CreatePermissionCommand("", 1);
            await Assert.ThrowsAsync<ArgumentException>(() => _createPermissionHandler.Handle(request, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_DuplicatePermission_ThrowsArgumentException()
        {
            var request = new CreatePermissionCommand("Test Permission", 1);
            var existingPermission = new Permission { Id = 1, Description = "Test Permission", PermissionTypeId = 1 };

            _unitOfWorkMock.Setup(u => u.Permissions.GetPermissionByDescriptionAsync(request.Description))
                .ReturnsAsync(existingPermission);

            await Assert.ThrowsAsync<ArgumentException>(() => _createPermissionHandler.Handle(request, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_PermissionTypeNotFound_ThrowsEntityNotFoundException()
        {
            var request = new CreatePermissionCommand("Test Permission", 1);

            _unitOfWorkMock.Setup(u => u.Permissions.GetPermissionByDescriptionAsync(request.Description))
                .ReturnsAsync((Permission)null);

            _unitOfWorkMock.Setup(u => u.PermissionsType.GetByIdAsync(request.PermissionTypeId))
                .ReturnsAsync((PermissionType)null);

            await Assert.ThrowsAsync<EntityNotFoundException>(() => _createPermissionHandler.Handle(request, CancellationToken.None));
        }
    }
}