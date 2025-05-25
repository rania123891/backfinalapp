using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageService.Domain.Models;

namespace MessageService.Data.Repositories
{
    public interface IMessageRepository
    {
        Task AddAsync(Message message);
        Task<List<Message>> GetMessagesByUserAsync(string userId);
    }
}
