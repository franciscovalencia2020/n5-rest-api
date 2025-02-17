using AutoMapper;
using Data.Models.DTOs.Permission.Response;
using Moq;
using Services.ElasticSearch.Interfaces;
using Services.Handlers.Permission;
using Services.Kafka.interfaces;
using Data.Models.DatabaseModels;

namespace Api.Tests
{
    public class GetPermissionsHandlerTests
    {
        private readonly Mock<IPermissionElasticService> _mockElasticService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IKafkaProducerService> _mockKafkaProducer;
        private readonly GetPermissionsHandler _handler;

        public GetPermissionsHandlerTests()
        {
            _mockElasticService = new Mock<IPermissionElasticService>();
            _mockMapper = new Mock<IMapper>();
            _mockKafkaProducer = new Mock<IKafkaProducerService>();

            _handler = new GetPermissionsHandler(_mockElasticService.Object, _mockMapper.Object, _mockKafkaProducer.Object);
        }

        [Fact]
        public async Task Handle_ReturnsListOfPermissions()
        {
            var permissions = new List<Permission> { new Permission() };
            _mockElasticService.Setup(e => e.GetAllPermissionsAsync()).ReturnsAsync(permissions);
            _mockMapper.Setup(m => m.Map<List<PermissionResponse>>(It.IsAny<IEnumerable<Permission>>())).Returns(new List<PermissionResponse> { new PermissionResponse() });

            var result = await _handler.Handle(new Services.Queries.Permission.GetPermissionsQuery(), default);

            Assert.NotNull(result);
            Assert.Single(result);
            _mockKafkaProducer.Verify(k => k.ProduceAsync("get", It.IsAny<string>()), Times.Once);
        }
    }
}
