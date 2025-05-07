using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetService.Domain.Models
{
    public enum RoleMembreEquipe
    {
        Membre,
        ChefEquipe
    }

    public class MembreEquipe
    {
        [Key]
        public int Id { get; set; }

        public int UtilisateurId { get; set; } // Géré par UserService

        // ✅ Garder uniquement `equipeId`
        public int EquipeId { get; set; }
        public Equipe? Equipe { get; set; } // 🔥 Nullable

        public RoleMembreEquipe Role { get; set; }
        public DateTime DateAjout { get; set; }
    }

}
