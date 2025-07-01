using MessageService.Domain.Interfaces;
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

    public async Task<Message?> GetByIdAsync(Guid messageId)
    {
        return await _context.Messages
            .Include(m => m.Attachments)
            .FirstOrDefaultAsync(m => m.Id == messageId);
    }

    public async Task<MessageAttachment?> GetAttachmentByIdAsync(Guid attachmentId)
    {
        return await _context.MessageAttachments.FindAsync(attachmentId);
    }

    public async Task DeleteAttachmentAsync(Guid attachmentId)
    {
        var attachment = await GetAttachmentByIdAsync(attachmentId);
        if (attachment != null)
        {
            _context.MessageAttachments.Remove(attachment);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<Message>> GetMessagesWithAttachmentsAsync(string userId)
    {
        return await _context.Messages
            .Include(m => m.Attachments)
            .Where(m => m.ExpediteurId == userId || m.DestinataireId == userId)
            .OrderByDescending(m => m.EnvoyeLe)
            .ToListAsync();
    }
}