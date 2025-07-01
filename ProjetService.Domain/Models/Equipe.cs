using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjetService.Domain.Models;
using ProjetService.Domain.Models.ProjetService.Domain.Models;

namespace ProjetService.Domain.Models
{

    public enum StatutEquipe
    {
        Active,
        Inactive
    }

    

    public class Equipe
    {
        [Key]
        public int IdEquipe { get; set; }

        [Required]
        public string Nom { get; set; }

        public StatutEquipe Statut { get; set; }

        public ICollection<ProjetEquipe> ProjetsEquipes { get; set; }

        // ✅ Relation avec MembresEquipe
        public ICollection<MembreEquipe>? MembresEquipe { get; set; }
        public ICollection<Tache> Taches { get; set; } = new List<Tache>();

    }

}
