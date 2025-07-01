using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageService.Domain.Models
{
    public class FileUploadResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Filename { get; set; } = string.Empty;
        public string OriginalName { get; set; } = string.Empty;
        public long Size { get; set; }
        public string MimeType { get; set; } = string.Empty;
        public DateTime UploadDate { get; set; }
        public string UploadedBy { get; set; } = string.Empty;
    }
}
