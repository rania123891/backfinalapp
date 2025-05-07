using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetService.Domain.Models
{
    public class Commentaire
    {
        [Key]
        public int Id { get; set; }

        public string Contenu { get; set; }

        public DateTime DateCreation { get; set; } = DateTime.UtcNow;

        public int UtilisateurId { get; set; } // Géré par UserService

        // ✅ Garder uniquement `tacheId`
        public int TacheId { get; set; }
        public Tache? Tache { get; set; } // 🔥 Nullable
    }


}
