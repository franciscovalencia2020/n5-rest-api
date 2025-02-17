namespace Data.Models.DTOs.User.Response
{
    public class UserResponse : BaseEntityDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? TokenExpiration { get; set; }
    }
}
