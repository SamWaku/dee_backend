namespace api.Dtos.User
{
    public class UserRequestDto
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public DateTime DateTime{ get; set; } = DateTime.UtcNow;
    }
}