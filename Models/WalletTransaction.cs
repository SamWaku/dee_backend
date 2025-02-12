namespace api.Models
{
    public class WalletTransaction
    {
        public int Id { get; set; }
        public string Title { get; set; }  = string.Empty;
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public Wallet? Wallet { get; set; } //Navigation property
        public int MyProperty { get; set; }
    }
}