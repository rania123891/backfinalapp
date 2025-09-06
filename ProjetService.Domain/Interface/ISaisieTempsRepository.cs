using ProjetService.Domain.DTO;
using ProjetService.Domain.Models;

namespace ProjetService.Domain.Interface
{
    public interface ISaisieTempsRepository
    {
        Task<SaisieTemps> GetByIdAsync(int id);
        Task<List<SaisieTemps>> GetByUtilisateurIdAsync(int utilisateurId, DateTime? dateDebut = null, DateTime? dateFin = null);
        Task<List<SaisieTemps>> GetByProjetIdAsync(int projetId, DateTime? dateDebut = null, DateTime? dateFin = null);
        Task<List<SaisieTemps>> GetByTacheIdAsync(int tacheId, DateTime? dateDebut = null, DateTime? dateFin = null);
        Task<int> CreateAsync(SaisieTemps saisieTemps);
        Task<bool> UpdateAsync(SaisieTemps saisieTemps);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<decimal> GetTotalHeuresParProjetAsync(int projetId, DateTime? dateDebut = null, DateTime? dateFin = null);
        Task<Dictionary<string, decimal>> GetHeuresParUtilisateurAsync(int projetId, DateTime? dateDebut = null, DateTime? dateFin = null);
        Task<Dictionary<string, decimal>> GetHeuresParTacheAsync(int projetId, DateTime? dateDebut = null, DateTime? dateFin = null);

        // ? NOUVELLES MÉTHODES avec noms
        Task<List<SaisieTempsDto>> GetSaisiesAvecNomsAsync(int projetId, DateTime? dateDebut = null, DateTime? dateFin = null);
        Task<List<PlanificationDto>> GetPlanificationsAvecNomsAsync(int projetId, DateTime? dateDebut = null, DateTime? dateFin = null);
    }
}