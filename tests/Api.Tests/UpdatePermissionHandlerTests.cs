using Moq;
using AutoMapper;
using Data.Models.DTOs.Permission.Response;
using Services.Handlers.Permission;
using Services.Commands.Permission;
using Services.ElasticSearch.Interfaces;
using Services.Kafka.interfaces;
using Data.Helpers;

namespace Api.Tests
{
    public class UpdatePermissionHandlerTests
    {
        private readonly Mock<IPermissionElasticService> _mockElasticService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IKafkaProducerService> _mockKafkaProducer;
        private readonly UpdatePermissionHandler _handler;

        public UpdatePermissionHandlerTests()
        {
            _mockElasticService = new Mock<IPermissionElasticService>();
            _mockMapper = new Mock<IMapper>();
            _mockKafkaProducer = new Mock<IKafkaProducerService>();

            _handler = new UpdatePermissionHandler(_mockElasticService.Object, _mockMapper.Object, _mockKafkaProducer.Object);
        }

        [Fact]
        public async Task Handle_ThrowsEntityNotFoundException_WhenPermissionDoesNotExist()
        {
            var command = new UpdatePermissionCommand(1, "Updated Permission", 1);
            _mockElasticService.Setup(e => e.GetPermissionByIdAsync(command.Id)).ReturnsAsync((Data.Models.DatabaseModels.Permission)null);

            await Assert.ThrowsAsync<EntityNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ThrowsArgumentException_WhenDescriptionAlreadyExists()
        {
            var command = new UpdatePermissionCommand(1, "Duplicate Permission", 1);
            var permission = new Data.Models.DatabaseModels.Permission { Id = 1, Description = "Old Permission" };
            var existingPermissions = new List<Data.Models.DatabaseModels.Permission> { new Data.Models.DatabaseModels.Permission { Id = 2, Description = "Duplicate Permission" } };

            _mockElasticService.Setup(e => e.GetPermissionByIdAsync(command.Id)).ReturnsAsync(permission);
            _mockElasticService.Setup(e => e.SearchPermissionsAsync(command.Description)).ReturnsAsync(existingPermissions);

            await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_UpdatesPermissionSuccessfully()
        {
            var command = new UpdatePermissionCommand(1, "Updated Permission", 1);
            var permission = new Data.Models.DatabaseModels.Permission { Id = 1, Description = "Old Permission" };

            _mockElasticService.Setup(e => e.GetPermissionByIdAsync(command.Id)).ReturnsAsync(permission);
            _mockElasticService.Setup(e => e.SearchPermissionsAsync(command.Description)).ReturnsAsync(new List<Data.Models.DatabaseModels.Permission>());

            _mockMapper.Setup(m => m.Map(command, permission));
            _mockMapper.Setup(m => m.Map<PermissionResponse>(permission)).Returns(new PermissionResponse { Id = 1, Description = "Updated Permission" });

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal("Updated Permission", result.Description);
            _mockElasticService.Verify(e => e.UpdatePermissionAsync(permission), Times.Once);
        }
    }
}
