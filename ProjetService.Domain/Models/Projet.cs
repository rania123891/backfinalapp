using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ProjetService.Domain.Models.ProjetService.Domain.Models;

namespace ProjetService.Domain.Models
{
    public enum StatutProjet
    {
        EnCours,
        Terminé,
        Annulé
    }

    public class Projet
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nom { get; set; }

        public string Description { get; set; }

        [Required]
        public StatutProjet Statut { get; set; }

        public DateTime DateDebut { get; set; }
        public DateTime DateEcheance { get; set; }
        public int Duree { get; set; }

        public int CreateurId { get; set; } // ID de l'utilisateur qui a créé le projet

        // ✅ Relation avec les entités liées (nullable)
        public ICollection<ProjetEquipe> ProjetsEquipes { get; set; }
        public virtual ICollection<Planification> Planifications { get; set; }

    }

}
