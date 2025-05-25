using MessageService.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace MessageService.Data.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly AppDbContext _context;

    public MessageRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Message message)
    {
        await _context.Messages.AddAsync(message);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Message>> GetMessagesByUserAsync(string userId)
    {
        return await _context.Messages
            .Where(m => m.ExpediteurId == userId || m.DestinataireId == userId)
            .OrderByDescending(m => m.EnvoyeLe)
            .ToListAsync();
    }
}