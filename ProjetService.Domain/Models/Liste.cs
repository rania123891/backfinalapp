using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetService.Domain.Models
{

    public class Liste
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nom { get; set; }

        public int Position { get; set; }

        public DateTime DateCreation { get; set; } = DateTime.UtcNow;

        // ✅ Garder uniquement `tableauId`
        public int TableauId { get; set; }
        public Tableau? Tableau { get; set; } // 🔥 Nullable

        // ✅ Relation avec les Tâches
        public ICollection<Tache>? Taches { get; set; }
    }

}
