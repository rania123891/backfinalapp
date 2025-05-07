using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetService.Domain.Models
{
    public enum PrioriteTache
    {
        Faible,
        Moyenne,
        Elevee
    }
    public enum StatutTache
    {
        EnCours,
        Terminee,
        Annulee
    }

    public class Tache
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Titre { get; set; }

        public string Description { get; set; }

        [Required]
        public StatutTache Statut { get; set; }

        [Required]
        public PrioriteTache Priorite { get; set; }

        public DateTime DateCreation { get; set; } = DateTime.UtcNow;
        public DateTime DateEcheance { get; set; }

        // ✅ Garder uniquement `projetId`
        public int ProjetId { get; set; }

        public Projet? Projet { get; set; }  // 🔥 Vérifier que ceci est `nullable`

        // ✅ Garder uniquement `listeId`
        public int ListeId { get; set; }

        public Liste? Liste { get; set; }  // 🔥 Vérifier que ceci est `nullable`

        public int AssigneId { get; set; }
        public ICollection<Commentaire>? Commentaires { get; set; } = new List<Commentaire>();

    }

}
