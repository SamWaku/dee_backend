using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using api.Data;

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
            var wallets = _context.Wallets.ToList(); //defered execution... sql completes the de
            return Ok(wallets);
        }

        [HttpGet({"id"})]
        public IActionResult GetById([FromRoute] int id)
        {

        }
    }
}