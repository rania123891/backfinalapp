using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageService.Domain.DTOs
{
    public class MessageDto
    {
        public Guid Id { get; set; }
        public string Contenu { get; set; }
        public string ExpediteurId { get; set; }  // Gardé en string
        public string DestinataireId { get; set; }
        public DateTime EnvoyeLe { get; set; }
        public UtilisateurDto Expediteur { get; set; }
        public UtilisateurDto Destinataire { get; set; }
    }
}
