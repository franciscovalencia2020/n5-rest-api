using Data.Helpers;
using Data.Models.DTOs.UserPermission.Request;
using Data.Models.DTOs.UserPermission.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UserPermissionController : ControllerBase
    {
        private readonly IUserPermissionService _userPermissionService;

        public UserPermissionController(IUserPermissionService userPermissionService)
        {
            _userPermissionService = userPermissionService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(UserPermissionResponse), 200)]
        public async Task<IActionResult> CreateUserPermission([FromBody] CreateUserPermissionRequest request)
        {
            return Ok(await _userPermissionService.CreateUserPermission(request));
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<UserPermissionResponse>), 200)]
        public async Task<IActionResult> GetUserPermissions()
        {
            return Ok(await _userPermissionService.GetUserPermissions());
        }

        [HttpGet("Paginated")]
        [ProducesResponseType(typeof(PaginatedList<UserPermissionResponse>), 200)]
        public async Task<IActionResult> GetUserPermissionsPaginated([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _userPermissionService.GetUserPermissionsPaginated(pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserPermissionResponse), 200)]
        public async Task<IActionResult> GetUserPermissionById(long id)
        {
            return Ok(await _userPermissionService.GetUserPermissionById(id));
        }

        [HttpGet("ByUserId/{id}")]
        [ProducesResponseType(typeof(List<UserPermissionResponse>), 200)]
        public async Task<IActionResult> GetUserPermissionByUserId(long id)
        {
            return Ok(await _userPermissionService.GetUserPermissionByUserId(id));
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(UserPermissionResponse), 200)]
        public async Task<IActionResult> UpdateUserPermission(long id, [FromBody] UpdateUserPermissionRequest request)
        {
            return Ok(await _userPermissionService.UpdateUserPermission(id, request));
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<IActionResult> DeleteUserPermission(long id)
        {
            return Ok(await _userPermissionService.DeleteUserPermission(id));
        }
    }
}
