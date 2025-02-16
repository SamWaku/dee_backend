using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace api.Models
{
    public class Wallet
    {
        public int Id { get; set; } 
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        
    }
}