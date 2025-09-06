using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetService.Domain.Models
{
    public class SaisieTemps
    {
        public int Id { get; set; }
        public int UtilisateurId { get; set; }
        public int ProjetId { get; set; }
        public int? TacheId { get; set; }
        public DateTime DateTravail { get; set; }
        public TimeSpan HeureDebut { get; set; }
        public TimeSpan HeureFin { get; set; }
        public decimal DureeHeures { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
