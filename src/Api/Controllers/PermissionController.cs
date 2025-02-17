using Data.Helpers;
using Data.Models.DTOs.Permission.Request;
using Data.Models.DTOs.Permission.Response;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Commands.Permission;
using Services.Queries.Permission;

namespace Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PermissionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PermissionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [ProducesResponseType(typeof(PermissionResponse), 200)]
        public async Task<IActionResult> CreatePermission([FromBody] CreatePermissionRequest request)
        {
            var command = new CreatePermissionCommand(request.Description, request.PermissionTypeId);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<PermissionResponse>), 200)]
        public async Task<IActionResult> GetPermissions()
        {
            var query = new GetPermissionsQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("Paginated")]
        [ProducesResponseType(typeof(PaginatedList<PermissionResponse>), 200)]
        public async Task<IActionResult> GetPermissionsPaginated([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var query = new GetPermissionsPaginatedQuery(pageNumber, pageSize);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PermissionResponse), 200)]
        public async Task<IActionResult> GetPermissionById(long id)
        {
            var query = new GetPermissionByIdQuery(id);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(PermissionResponse), 200)]
        public async Task<IActionResult> UpdatePermission(long id, [FromBody] UpdatePermissionRequest request)
        {
            var command = new UpdatePermissionCommand(id, request.Description, request.PermissionTypeId);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<IActionResult> DeletePermission(long id)
        {
            var command = new DeletePermissionCommand(id);
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
