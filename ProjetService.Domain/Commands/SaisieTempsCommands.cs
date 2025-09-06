using MediatR;
using ProjetService.Domain.DTO;

namespace ProjetService.Domain.Commands
{
    public class CreateSaisieTempsCommand : IRequest<int>
    {
        public int UtilisateurId { get; set; }
        public int ProjetId { get; set; }
        public int? TacheId { get; set; }
        public DateTime DateTravail { get; set; }
        public TimeSpan HeureDebut { get; set; }
        public TimeSpan HeureFin { get; set; }
        public decimal DureeHeures { get; set; }
        public string Description { get; set; }
    }

    public class UpdateSaisieTempsCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public int? TacheId { get; set; }
        public DateTime DateTravail { get; set; }
        public TimeSpan HeureDebut { get; set; }
        public TimeSpan HeureFin { get; set; }
        public decimal DureeHeures { get; set; }
        public string Description { get; set; }
    }

    public class DeleteSaisieTempsCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public int UtilisateurId { get; set; } // Pour vérifier que l'utilisateur peut supprimer
    }
} 