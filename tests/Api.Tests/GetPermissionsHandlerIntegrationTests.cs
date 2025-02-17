using AutoMapper;
using Data.Models.DatabaseModels;
using Data.Models.DTOs.Permission.Response;
using Moq;
using Services.ElasticSearch.Interfaces;
using Services.Handlers.Permission;
using Services.Kafka.interfaces;
using Services.Queries.Permission;

namespace Api.Tests
{
    public class GetPermissionsHandlerIntegrationTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IPermissionElasticService> _elasticServiceMock;
        private readonly Mock<IKafkaProducerService> _kafkaProducerMock;
        private readonly GetPermissionsHandler _getPermissionsHandler;

        public GetPermissionsHandlerIntegrationTests()
        {
            _mapperMock = new Mock<IMapper>();
            _elasticServiceMock = new Mock<IPermissionElasticService>();
            _kafkaProducerMock = new Mock<IKafkaProducerService>();

            _getPermissionsHandler = new GetPermissionsHandler(
                _elasticServiceMock.Object,
                _mapperMock.Object,
                _kafkaProducerMock.Object
            );
        }

        [Fact]
        public async Task Handle_GetPermissions_ReturnsListOfPermissions()
        {
            var permissions = new List<Permission>
            {
                new Permission { Id = 1, Description = "Permission 1", PermissionTypeId = 1 },
                new Permission { Id = 2, Description = "Permission 2", PermissionTypeId = 2 }
            };

            var permissionResponses = new List<PermissionResponse>
            {
                new PermissionResponse { Id = 1, Description = "Permission 1", PermissionTypeId = 1 },
                new PermissionResponse { Id = 2, Description = "Permission 2", PermissionTypeId = 2 }
            };

            _elasticServiceMock.Setup(e => e.GetAllPermissionsAsync())
                .ReturnsAsync(permissions);

            _mapperMock.Setup(m => m.Map<List<PermissionResponse>>(permissions))
                .Returns(permissionResponses);

            _kafkaProducerMock.Setup(k => k.ProduceAsync("get", It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var result = await _getPermissionsHandler.Handle(new GetPermissionsQuery(), CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Permission 1", result[0].Description);
            Assert.Equal("Permission 2", result[1].Description);

            _elasticServiceMock.Verify(e => e.GetAllPermissionsAsync(), Times.Once);
            _kafkaProducerMock.Verify(k => k.ProduceAsync("get", It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task Handle_GetPermissions_SendsKafkaMessage()
        {
            var permissions = new List<Permission>
            {
                new Permission { Id = 1, Description = "Permission 1", PermissionTypeId = 1 }
            };

            _elasticServiceMock.Setup(e => e.GetAllPermissionsAsync())
                .ReturnsAsync(permissions);

            _mapperMock.Setup(m => m.Map<List<PermissionResponse>>(permissions))
                .Returns(new List<PermissionResponse>());

            _kafkaProducerMock.Setup(k => k.ProduceAsync("get", It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            await _getPermissionsHandler.Handle(new GetPermissionsQuery(), CancellationToken.None);

            _kafkaProducerMock.Verify(k => k.ProduceAsync("get", It.IsAny<string>()), Times.Once);
        }
    }
}
