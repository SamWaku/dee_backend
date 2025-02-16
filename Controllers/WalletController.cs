using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using api.Data;
using api.Mappers;
using api.Dtos.Wallet;
using Microsoft.EntityFrameworkCore;
using api.Interfaces;
using api.Models;

namespace api.Controllers
{
    [Route("api/wallet")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly ApplicationDBContext _context; //prevent being changed
        private readonly IWalletRepository _walletRepo;
        public WalletController(ApplicationDBContext context, IWalletRepository walletRepo)
        {
            _walletRepo = walletRepo;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var wallets = await _walletRepo.GetAllAsync();
            var walletDto = wallets.Select(w => w.ToWalletDto()); //defered execution... sql completes the fetch. Additionally we have mapped the DTO here
            return Ok(wallets);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id) //model binding
        {
            var wallet = await _context.Wallets.FindAsync(id);

            if (wallet == null)
            {
                return NotFound();
            }

            return Ok(wallet.ToWalletDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateWalletRequestDto walletDto)
        {
            var walletModel = walletDto.ToWalletFromCreateDto();
            await _context.Wallets.AddAsync(walletModel);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new {id = walletModel.Id}, walletModel.ToWalletDto());
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateWalletRequestDto updateWalletDto)
        {
            var walletModel = await _context.Wallets.FirstOrDefaultAsync(w => w.Id == id);

            if (walletModel == null)
            {
                return NotFound();
            }

            walletModel.Amount = updateWalletDto.Amount;
            await _context.SaveChangesAsync();
            return Ok(walletModel.ToWalletDto());
        }

        [HttpDelete]
        [Route("{id}")]
        public async  Task<IActionResult> Delete([FromRoute] int id)
        {
            var walletModel = _context.Wallets.FirstOrDefault(x => x.Id == id);

            if(walletModel == null)
            {
                return NotFound();
            }

            _context.Wallets.Remove(walletModel); //dont add an await to the remove function
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost]
        [Route("transfer")]
        public async Task<IActionResult> TransferFunds([FromBody] Dtos.WalletTransaction.WalletTransferRequestDto transferDto)
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