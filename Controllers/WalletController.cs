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
        private readonly ApplicationDBContext _context;
        public WalletController(ApplicationDBContext context)
        {
            
        }
    }
}