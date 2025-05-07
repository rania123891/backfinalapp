using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetService.Domain.Models
{

    public enum StatutEquipe
    {
        Active,
        Inactive
    }

    public enum Domaine
    {
        FrontEnd,
        BackEnd,
        BaseDonnee
    }

    public class Equipe
    {
        [Key]
        public int IdEquipe { get; set; }

        [Required]
        public string Nom { get; set; }

        public StatutEquipe Statut { get; set; }
        public Domaine DomaineActivite { get; set; }

        // ✅ Garder uniquement `projetId`
        public int ProjetId { get; set; }
        public Projet? Projet { get; set; } // 🔥 Nullable

        // ✅ Relation avec MembresEquipe
        public ICollection<MembreEquipe>? MembresEquipe { get; set; }
    }

}
