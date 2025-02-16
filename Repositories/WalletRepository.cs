using api.Interfaces;
using api.Models;

namespace api.Repositories
{
    public class WalletRepository : IWalletRepository
    {        
        public Task<List<Wallet>> GetAllAsync()
        {
            throw new NotImplementedException();
        }
    }
}