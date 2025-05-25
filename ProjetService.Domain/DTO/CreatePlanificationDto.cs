using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjetService.Domain.Models;

namespace ProjetService.Domain.DTO
{
    public class CreatePlanificationDto
    {
        public DateTime Date { get; set; }
        public string HeureDebut { get; set; }
        public string HeureFin { get; set; }
        public string Description { get; set; }
        public int TacheId { get; set; }
        public int ProjetId { get; set; }
        public EtatListe ListeId { get; set; } = EtatListe.Todo;
    }

    public class UpdatePlanificationDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string HeureDebut { get; set; }
        public string HeureFin { get; set; }
        public string Description { get; set; }
        public int TacheId { get; set; }
        public int ProjetId { get; set; }
        public EtatListe ListeId { get; set; }
    }

    public class UpdatePlanificationStatusDto
    {
        public EtatListe ListeId { get; set; }
    }
}
