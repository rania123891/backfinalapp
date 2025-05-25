using System;
using System.ComponentModel.DataAnnotations;
using MediatR;
using ProjetService.Domain.Models;

namespace ProjetService.Domain.Commands
{
    public class CreateProjetCommand : IRequest<Projet>
    {
        [Required]
        [StringLength(200)]
        public string Nom { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [Required]
        public DateTime DateDebut { get; set; }

        [Required]
        public DateTime DateEcheance { get; set; }

        public StatutProjet Statut { get; set; } = StatutProjet.EnCours;

        [Required]
        public int CreateurId { get; set; }

        public int? Duree => (DateEcheance - DateDebut).Days;
    }

    public class UpdateProjetCommand : IRequest<Projet>
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Nom { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [Required]
        public DateTime DateDebut { get; set; }

        [Required]
        public DateTime DateEcheance { get; set; }

        public StatutProjet Statut { get; set; }

        [Required]
        public int CreateurId { get; set; }
    }

    public class DeleteProjetCommand : IRequest<bool>
    {
        [Required]
        public int Id { get; set; }
    }
}