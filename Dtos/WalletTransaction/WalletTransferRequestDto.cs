namespace api.Dtos.WalletTransaction
{
    public class WalletTransferRequestDto
    {
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public decimal Amount { get; set; }
    }
}