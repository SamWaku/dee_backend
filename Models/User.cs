using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace api.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string? Username { get; set; }
        [Required]
        public string? Email { get; set; }
        [Required]
         [JsonIgnore]
        public string? Password { get; set; }
        public DateTime DateTime{ get; set; } = DateTime.UtcNow;

    }
}