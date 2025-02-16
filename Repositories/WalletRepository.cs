using api.Data;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repositories
{
    public class WalletRepository : IWalletRepository
    {        
        private readonly ApplicationDBContext _context;
        //dependency injection
        public WalletRepository(ApplicationDBContext context){
            _context = context;
        }

        public Task<List<Wallet>> GetAllAsync()
        {
            return _context.Wallets.ToListAsync();
        }
    }
}