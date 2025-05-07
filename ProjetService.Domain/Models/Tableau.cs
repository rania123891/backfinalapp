using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetService.Domain.Models
{
    public class Tableau
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nom { get; set; }

        public string Description { get; set; }

        public int Position { get; set; }

        // ✅ Garder uniquement `projetId`
        public int ProjetId { get; set; }
        public Projet? Projet { get; set; } // 🔥 Nullable pour éviter les erreurs

        // ✅ Relation avec Listes
        public ICollection<Liste>? Listes { get; set; }
    }

}
