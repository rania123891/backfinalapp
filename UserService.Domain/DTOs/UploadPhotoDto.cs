using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace UserService.Domain.DTOs
{
    public class UploadPhotoDto
    {
        [Required]
        public IFormFile Photo { get; set; }

        // 🚧 Temporaire pour debug
        public int? UserId { get; set; }
    }

}
