using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Domain.Interfaces.Infrastructure
{
    public interface IEmailVerificationService
    {
        string GenerateToken();
        Task SendVerificationEmail(string email, string token);
        Task<bool> VerifyEmail(string email, string token);
    }
}
