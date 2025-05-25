using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageService.Domain.Models
{
    public class Message
    {
        public Guid Id { get; set; }
        public string Contenu { get; set; }
        public string ExpediteurId { get; set; }  // Changé en string pour correspondre à l'ID utilisateur
        public string DestinataireId { get; set; }
        public DateTime EnvoyeLe { get; set; }
    }

}
