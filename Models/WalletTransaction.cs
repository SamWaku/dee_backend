using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    public class WalletTransaction
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }  = string.Empty;
        public int WalletId { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
    }
}