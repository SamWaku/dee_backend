using api.Models;

namespace api.Interfaces
{
    public interface IWalletTransactionRepository
    {
         Task<List<WalletTransaction>> GetAllAsync();
    }
}