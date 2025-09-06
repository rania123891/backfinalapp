using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Domain.Interfaces.Infrastructure
{
    public interface IEmailService
    {
        Task SendVerificationEmail(string email, string verificationLink);
    }
}
