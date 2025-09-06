using System;
using System.ComponentModel.DataAnnotations;

namespace ProjetService.Domain.DTO
{
    public class CreateSaisieTempsDto
    {
        [Required]
        public int UtilisateurId { get; set; }

        [Required]
        public int ProjetId { get; set; }

        public int? TacheId { get; set; }

        [Required]
        public DateTime DateTravail { get; set; }

        [Required]
        public TimeSpan HeureDebut { get; set; }

        [Required]
        public TimeSpan HeureFin { get; set; }

        [Range(0.1, 24.0)]
        public decimal DureeHeures { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }
    }

    public class UpdateSaisieTempsDto
    {
        [Required]
        public int Id { get; set; }

        public int? TacheId { get; set; }

        [Required]
        public DateTime DateTravail { get; set; }

        [Required]
        public TimeSpan HeureDebut { get; set; }

        [Required]
        public TimeSpan HeureFin { get; set; }

        [Range(0.1, 24.0)]
        public decimal DureeHeures { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }
    }

    public class SaisieTempsDto
    {
        public int Id { get; set; }
        public int UtilisateurId { get; set; }
        public int ProjetId { get; set; }
        public int? TacheId { get; set; }
        public DateTime DateTravail { get; set; }
        public TimeSpan HeureDebut { get; set; }
        public TimeSpan HeureFin { get; set; }
        public decimal DureeHeures { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }

        // ✅ Nouveaux champs avec les noms
        public string NomProjet { get; set; }
        public string NomTache { get; set; }
        public string NomUtilisateur { get; set; }
    }

    public class PlanificationDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string HeureDebut { get; set; }
        public string HeureFin { get; set; }
        public decimal DureeHeures { get; set; }
        public string Description { get; set; }
        public int TacheId { get; set; }
        public int ProjetId { get; set; }
        public int UserId { get; set; }

        // ✅ Champs avec les noms
        public string NomProjet { get; set; }
        public string NomTache { get; set; }
        public string NomUtilisateur { get; set; }
    }
    public class FicheHeureResult
    {
        public int ProjetId { get; set; }
        public string NomProjet { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }
        public List<SaisieTempsDto> SaisiesTemps { get; set; } = new List<SaisieTempsDto>();
        public decimal TotalHeures { get; set; }
        public Dictionary<string, decimal> HeuresParUtilisateur { get; set; } = new Dictionary<string, decimal>();
        public Dictionary<string, decimal> HeuresParTache { get; set; } = new Dictionary<string, decimal>();
    }
} 