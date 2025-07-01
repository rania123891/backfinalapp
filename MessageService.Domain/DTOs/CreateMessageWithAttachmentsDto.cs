using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http; // Pour IFormFile

namespace MessageService.Domain.DTOs
{
    public class CreateMessageWithAttachmentsDto
    {
        public string Contenu { get; set; }
        public string ExpediteurEmail { get; set; }
        public string EmailDestinataire { get; set; }
        public string? ExpediteurId { get; set; }
        public string? DestinataireId { get; set; }

        // ✅ NOUVEAU : Liste des fichiers
        public List<IFormFile>? Attachments { get; set; }
    }
}
