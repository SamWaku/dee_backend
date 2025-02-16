using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using api.Data;
using api.Mappers;
using api.Dtos.Wallet;

namespace api.Controllers
{
    [Route("api/wallet")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly ApplicationDBContext _context; //prevent being changed
        public WalletController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var wallets = _context.Wallets.ToList()
            .Select(w => w.ToWalletDto()); //defered execution... sql completes the fetch. Additionally we have mapped the DTO here
            return Ok(wallets);
        }

        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute] int id) //model binding
        {
            var wallet = _context.Wallets.Find(id);

            if (wallet == null)
            {
                return NotFound();
            }

            return Ok(wallet.ToWalletDto());
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateWalletRequestDto walletDto)
        {
            var walletModel = walletDto.ToWalletFromCreateDto();
            _context.Wallets.Add(walletModel);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetById), new {id = walletModel.Id}, walletModel.ToWalletDto());
        }

        [HttpPut]
        [Route("{id}")]
        public IActionResult Update([FromRoute] int id, [FromBody] UpdateWalletRequestDto updateWalletDto)
        {
            var walletModel = _context.Wallets.FirstOrDefault(w => w.Id == id);

            if (walletModel == null)
            {
                return NotFound();
            }

            walletModel.Amount = updateWalletDto.Amount;
            _context.SaveChanges();
            return Ok(walletModel.ToWalletDto());
        }

        [HttpDelete]
        [Route("{id}")]
        public IActionResult Delete([FromRoute] int id)
        {
            var walletModel = _context.Wallets.FirstOrDefault(x => x.Id == id);

            if(walletModel == null)
            {
                return NotFound();
            }

            _context.Wallets.Remove(walletModel);
            _context.SaveChanges();
            return NoContent();
        }
    }
}