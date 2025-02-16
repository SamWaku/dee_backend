namespace api.Dtos.WalletTransaction
{
    public class WalletTransactionDto
    {
        public int Id { get; set; }
        public decimal TransactionAmount { get; set; }
        public string? SenderId { get; set; } 
        public string? RecieverId { get; set; }
        public string? WalletId { get; set; }
    }
}