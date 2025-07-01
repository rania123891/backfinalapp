using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageService.Domain.Models
{
  
    
        public class FileUpload
        {
            [Key]
            [Required]
            [MaxLength(255)]
            public string Id { get; set; } = string.Empty;

            [Required]
            [MaxLength(255)]
            public string OriginalName { get; set; } = string.Empty;

            [Required]
            [MaxLength(255)]
            public string StoredName { get; set; } = string.Empty;

            [Required]
            [MaxLength(500)]
            public string FilePath { get; set; } = string.Empty;

            [Required]
            [MaxLength(100)]
            public string MimeType { get; set; } = string.Empty;

            public long Size { get; set; }

            public DateTime UploadDate { get; set; }

            [Required]
            [MaxLength(50)]
            public string UploadedBy { get; set; } = string.Empty;
        }
    
}
