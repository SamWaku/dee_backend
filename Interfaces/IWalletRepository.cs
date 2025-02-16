using api.Models;

namespace api.Interfaces
{
    public interface IWalletRepository
    {
        Task<List<Wallet>> GetAllAsync();
    }
}