using Data.Helpers;
using Data.Models.DTOs.PermissionType.Request;
using Data.Models.DTOs.PermissionType.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PermissionTypeController : ControllerBase
    {
        private readonly IPermissionTypeService _permissionTypeService;

        public PermissionTypeController(IPermissionTypeService permissionTypeService)
        {
            _permissionTypeService = permissionTypeService;
        }

        [HttpPost()]
        [ProducesResponseType(typeof(PermissionTypeResponse), 200)]
        public async Task<IActionResult> CreatePermissionType([FromBody] CreatePermissionTypeRequest request)
        {
            return Ok(await _permissionTypeService.CreatePermissionType(request));
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<PermissionTypeResponse>), 200)]
        public async Task<IActionResult> GetPermissionsType()
        {
            return Ok(await _permissionTypeService.GetPermissionsType());
        }

        [HttpGet("Paginated")]
        [ProducesResponseType(typeof(PaginatedList<PermissionTypeResponse>), 200)]
        public async Task<IActionResult> GetPermissionsTypePaginated([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _permissionTypeService.GetPermissionsTypePaginated(pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PermissionTypeResponse), 200)]
        public async Task<IActionResult> GetPermissionTypeById(long id)
        {
            return Ok(await _permissionTypeService.GetPermissionTypeById(id));
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(PermissionTypeResponse), 200)]
        public async Task<IActionResult> UpdatePermissionType(long id, [FromBody] UpdatePermissionTypeRequest request)
        {
            return Ok(await _permissionTypeService.UpdatePermissionType(id, request));
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<IActionResult> DeletePermissionType(long id)
        {
            return Ok(await _permissionTypeService.DeletePermissionType(id));
        }
    }
}
