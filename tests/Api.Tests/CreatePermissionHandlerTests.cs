using Moq;
using AutoMapper;
using Data.Interfaces;
using Data.Models.DTOs.Permission.Response;
using Services.Handlers.Permission;
using Services.Commands.Permission;
using Services.ElasticSearch.Interfaces;
using Services.Kafka.interfaces;
using Data.Models.DatabaseModels;

namespace Api.Tests
{
    public class CreatePermissionHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IPermissionElasticService> _mockElasticService;
        private readonly Mock<IKafkaProducerService> _mockKafkaProducer;
        private readonly CreatePermissionHandler _handler;

        public CreatePermissionHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _mockElasticService = new Mock<IPermissionElasticService>();
            _mockKafkaProducer = new Mock<IKafkaProducerService>();

            _handler = new CreatePermissionHandler(_mockUnitOfWork.Object, _mockMapper.Object, _mockElasticService.Object, _mockKafkaProducer.Object);
        }

        [Fact]
        public async Task Handle_ThrowsArgumentException_WhenDescriptionIsNull()
        {
            var command = new CreatePermissionCommand(null, 1);
            await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_CreatesPermissionSuccessfully()
        {
            var command = new CreatePermissionCommand("Test Permission", 1);
            var permission = new Permission { Id = 1, Description = "Test Permission" };
            var response = new PermissionResponse { Id = 1, Description = "Test Permission" };

            _mockUnitOfWork.Setup(u => u.Permissions.GetPermissionByDescriptionAsync(command.Description)).ReturnsAsync((Permission)null);
            _mockUnitOfWork.Setup(u => u.PermissionsType.GetByIdAsync(command.PermissionTypeId)).ReturnsAsync(new PermissionType());
            _mockMapper.Setup(m => m.Map<Permission>(command)).Returns(permission);
            _mockMapper.Setup(m => m.Map<PermissionResponse>(permission)).Returns(response);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(response.Description, result.Description);
            _mockElasticService.Verify(e => e.IndexPermissionAsync(permission), Times.Once);
            _mockKafkaProducer.Verify(k => k.ProduceAsync("request", It.IsAny<string>()), Times.Once);
        }
    }
}