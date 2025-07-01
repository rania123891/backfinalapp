using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageService.Domain.DTOs
{
    public class MessageWithAttachmentsDto
    {
        public Guid Id { get; set; }
        public string Contenu { get; set; }
        public string ExpediteurId { get; set; }
        public string DestinataireId { get; set; }
        public DateTime EnvoyeLe { get; set; }
        public List<MessageAttachmentDto> Attachments { get; set; } = new();
    }
}
