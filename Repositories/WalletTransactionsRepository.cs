using api.Data;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repositories
{
    public class WalletTransactionRepository : IWalletTransactionRepository
    {
        private readonly ApplicationDBContext _context;

        public WalletTransactionRepository(ApplicationDBContext context){
            _context = context;
        }
        public Task<List<WalletTransaction>> GetAllAsync()
        {
            return _context.WalletTransactions.ToListAsync();
        }
    }
}