using api.Data;
using api.Dtos.WalletTransaction;
using api.Interfaces;
using api.Migrations;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CsvHelper;
using System.Globalization;

namespace api.Controllers
{
    [Route("api/wallet-transactions")]
    [ApiController]
    public class WalletTransactionController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        private readonly IWalletTransactionRepository _walletTransactionRepo;

        public WalletTransactionController(ApplicationDBContext context, IWalletTransactionRepository walletTransaction)
        {
            _walletTransactionRepo = walletTransaction;
            _context = context;
        } 

        [HttpPost]
        // [Route("transfer")]
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

        [HttpGet("wallet-transactions/{userId}")]
        public async Task<IActionResult> GetUserTransactions(int userId)
        {
            var transactions = await _context.WalletTransactions
                .Where(t => t.SenderId == userId.ToString() || t.RecieverId == userId.ToString())
                .Select(t => new WalletTransactionDto
                {
                    Id = t.Id,
                    TransactionAmount = t.TransactionAmount,
                    SenderId = t.SenderId,
                    RecieverId = t.RecieverId,
                    WalletId = t.WalletId
                })
                .ToListAsync();

            if (!transactions.Any())
            {
                return NotFound(new { message = "No transactions found for this user." });
            }

            return Ok(transactions);
        }

        [HttpGet]
        [Route("export")]
        [ProducesResponseType(typeof(FileResult), 200)]
        public async Task<IActionResult> ExportUserTransactions()
        {
            var transactions = await _context.WalletTransactions.ToListAsync();

            var memoryStream = new MemoryStream();
            using (var streamWriter = new StreamWriter(memoryStream, leaveOpen: true))
            using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
            {
                csvWriter.WriteRecords(transactions);
                await streamWriter.FlushAsync();
            }

            memoryStream.Position = 0;

            return File(memoryStream, "text/csv", "wallet_transactions.csv");
        }


    }
}