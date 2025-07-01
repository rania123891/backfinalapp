// CreateTacheCommand.cs
using System;
using System.ComponentModel.DataAnnotations;
using MediatR;
using ProjetService.Domain.Models;

namespace ProjetService.Domain.Commands
{
    public class CreateTacheCommand : IRequest<Tache>
    {
        [Required]
        [StringLength(200)]
        public string Titre { get; set; }  // ✅ Utiliser "Titre" comme votre modèle

        [Required]
        public PrioriteTache Priorite { get; set; } = PrioriteTache.Moyenne;  // ✅ Enum

        [Required]
        public int EquipeId { get; set; }  // ✅ ID de l'équipe
    }
}