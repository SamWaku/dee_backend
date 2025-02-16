using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using api.Data;
using api.Mappers;

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

            return Ok(wallet);
        }
    }
}