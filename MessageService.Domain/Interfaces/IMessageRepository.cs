using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MessageService.Domain.Models;



namespace MessageService.Domain.Interfaces
{
    public interface IMessageRepository
    {
        Task AddAsync(Message message);
        Task<List<Message>> GetMessagesByUserAsync(string userId);
        Task<Message?> GetByIdAsync(Guid messageId);
        Task<MessageAttachment?> GetAttachmentByIdAsync(Guid attachmentId);
        Task DeleteAttachmentAsync(Guid attachmentId);
        Task<List<Message>> GetMessagesWithAttachmentsAsync(string userId);
    }
}
