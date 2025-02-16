namespace api.Dtos.Wallet
{
    public class WalletDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
    
    }
}