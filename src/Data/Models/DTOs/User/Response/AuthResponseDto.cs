namespace Data.Models.DTOs.User.Response
{
    public class AuthResponseDto
    {
        public UserResponse User { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
