using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Domain.Models
{
  
    public class EmailVerification
    {
        public int Id { get; set; }
        public string Email { get; set; }  // Pas de clé étrangère pour simplifier
        public string Token { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
