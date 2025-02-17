using Data.Helpers;
using Data.Models.DTOs.User.Request;
using Data.Models.DTOs.User.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResponseDto), 200)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            return Ok(await _userService.LoginAsync(request.Email, request.Password));
        }

        [HttpPost("RefreshToken")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResponseDto), 200)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            return Ok(await _userService.RefreshTokenAsync(request));
        }

        [HttpPost()]
        [ProducesResponseType(typeof(UserResponse), 200)]
        [AllowAnonymous]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            return Ok(await _userService.CreateUser(request));
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<UserResponse>), 200)]
        public async Task<IActionResult> GetUsers()
        {
            return Ok(await _userService.GetUsers());
        }

        [HttpGet("Paginated")]
        [ProducesResponseType(typeof(PaginatedList<UserResponse>), 200)]
        public async Task<IActionResult> GetUsersPaginated([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _userService.GetUsersPaginated(pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserResponse), 200)]
        public async Task<IActionResult> GetUserById(long id)
        {
            return Ok(await _userService.GetUserById(id));
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(UserResponse), 200)]
        public async Task<IActionResult> UpdateUser(long id, [FromBody] UpdateUserRequest request)
        {
            return Ok(await _userService.UpdateUser(id, request));
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<IActionResult> DeleteUser(long id)
        {
            return Ok(await _userService.DeleteUser(id));
        }
    }
}
