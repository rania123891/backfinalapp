using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageService.Domain.DTOs
{
    public class MessageAttachmentDto
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string OriginalFileName { get; set; }
        public string FileType { get; set; }
        public string MimeType { get; set; }
        public long FileSize { get; set; }
        public DateTime UploadedAt { get; set; }
        public string DownloadUrl { get; set; } // URL pour télécharger le fichier
    }

}
