using api.Data;
using api.Dtos.WalletTransaction;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    public class WalletTransactionController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public WalletTransactionController(ApplicationDBContext context) => _context = context;

                [HttpPost]
        [Route("transfer")]
        public async Task<IActionResult> TransferFunds([FromBody] WalletTransferRequestDto transferDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                // 1. Fetch sender and receiver wallets
                var senderWallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == transferDto.SenderId);
                var receiverWallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == transferDto.ReceiverId);

                if (senderWallet == null || receiverWallet == null)
                {
                    return BadRequest(new { message = "Sender or receiver wallet not found." });
                }

                // 2. Validate sender balance
                if (senderWallet.Amount < transferDto.Amount)
                {
                    return BadRequest(new { message = "Insufficient balance." });
                }

                // 3. Perform transfer
                senderWallet.Amount -= transferDto.Amount;
                receiverWallet.Amount += transferDto.Amount;

                // 4. Create transaction record
                WalletTransaction transactionRecord = new()
                {
                    TransactionAmount = transferDto.Amount,
                    SenderId = transferDto.SenderId.ToString(),
                    RecieverId = transferDto.ReceiverId.ToString(),
                    WalletId = senderWallet.Id.ToString(),
                    CreatedOn = DateTime.UtcNow
                };

                // 5. Save to database
                _context.WalletTransactions.Add(transactionRecord);
                await _context.SaveChangesAsync();
                
                await transaction.CommitAsync(); // Commit transaction

                return Ok(new { message = "Transfer successful." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(); // Rollback in case of error
                return StatusCode(500, new { message = "An error occurred.", error = ex.Message });
            }
        }

    }
}