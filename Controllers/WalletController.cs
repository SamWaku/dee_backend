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
using api.Dtos.WalletTransaction;
using System.Globalization; 
using System.IO;
using CsvHelper;

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
            
            var walletDtos = wallets.Select(w => new WalletResponseDto
            {
                Id = w.Id,
                Amount = w.Amount,
                UserId = w.UserId
            });

            return Ok(walletDtos);
        }


        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId([FromRoute] int userId)
        {
            var wallet = await _context.Wallets
                .Where(w => w.UserId == userId)
                .Select(w => new WalletResponseDto
                {
                    Id = w.Id,
                    Amount = w.Amount,
                    UserId = w.UserId
                })
                .FirstOrDefaultAsync();

            if (wallet == null)
            {
                return NotFound(new { message = "No wallet found for this user." });
            }

            return Ok(wallet);
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

        [HttpGet]
        [Route("export")]
        [ProducesResponseType(typeof(FileResult), 200)]
        public async Task<IActionResult> ExportWalletReport()
        {
            var wallets = await _context.Wallets.ToListAsync();

            using var memoryStream = new MemoryStream();
            using var streamWriter = new StreamWriter(memoryStream);
            using var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture);
            

            csvWriter.WriteRecords(wallets);

            await streamWriter.FlushAsync();
            memoryStream.Position = 0;

            return File(memoryStream, "text/csv", "wallet_report.csv");
        }

        // [HttpGet]
        // public async Task<IActionResult> GetAll()
        // {
        //     var wallets = await _walletRepo.GetAllAsync();
        //     var walletDto = wallets.Select(w => w.ToWalletDto()); //defered execution... sql completes the fetch. Additionally we have mapped the DTO here
        //     return Ok(wallets);
        // }

        // [HttpGet("{id}")]
        // public async Task<IActionResult> GetById([FromRoute] int id) //model binding
        // {
        //     var wallet = await _context.Wallets.FindAsync(id);

        //     if (wallet == null)
        //     {
        //         return NotFound();
        //     }

        //     return Ok(wallet.ToWalletDto());
        // }

        // [HttpPost]
        // public async Task<IActionResult> Create([FromBody] Dtos.Wallet.CreateWalletRequestDto walletDto)
        // {
        //     var walletModel = walletDto.ToWalletFromCreateDto();
        //     await _context.Wallets.AddAsync(walletModel);
        //     await _context.SaveChangesAsync();
        //     return CreatedAtAction(nameof(GetById), new {id = walletModel.Id}, walletModel.ToWalletDto());
        // }
    }
}