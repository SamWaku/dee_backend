using api.Interfaces;
using api.Models;

namespace api.Repositories
{
    public class WalletTransactionRepository : IWalletTransactionRepository
    {
        public Task<List<WalletTransaction>> GetAllAsync()
        {
            throw new NotImplementedException();
        }
    }
}