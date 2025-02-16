using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    public class WalletTransaction
    {
        [Key]
        public int Id { get; set; }
        public decimal TransactionAmount { get; set; }
        public string? SenderId { get; set; } 
        public string? RecieverId { get; set; }
        public string? WalletId { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
    }
}