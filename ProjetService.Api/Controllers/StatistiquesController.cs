using Microsoft.AspNetCore.Mvc;
using ProjetService.Domain.Interface;
using ProjetService.Domain.Interfaces;
using ProjetService.Domain.Models;

namespace ProjetService.Api.Controllers
{
    [Route("api/statistiques")]
    [ApiController]
    public class StatistiquesController : ControllerBase
    {
        private readonly IGenericRepository<Tache> _tacheRepository;
        private readonly IGenericRepository<Projet> _projetRepository;
        private readonly IPlanificationRepository _planificationRepository;

        public StatistiquesController(
            IGenericRepository<Projet> projetRepository,
            IGenericRepository<Tache> tacheRepository,
            IPlanificationRepository planificationRepository)
        {
            _projetRepository = projetRepository;
            _tacheRepository = tacheRepository;
            _planificationRepository = planificationRepository;
        }

        [HttpGet]
        public async Task<ActionResult<object>> GetStatistiques()
        {
            try
            {
                // Récupérer les données de base
                var projets = await _projetRepository.GetAllAsync();
                var taches = await _tacheRepository.GetAllAsync();
                var planifications = await _planificationRepository.GetAllAsync();

                // Calculer les statistiques
                var statistiques = new
                {
                    totalProjets = projets.Count(),
                    totalTaches = taches.Count(),
                    totalUtilisateurs = 6, // Valeur fixe pour le moment
                    totalEquipes = 0, // Valeur fixe pour le moment
                    tachesParStatut = new
                    {
                        // Utiliser les statuts de projet comme approximation
                        EnCours = projets.Count(p => p.Statut == StatutProjet.EnCours),
                        Termine = projets.Count(p => p.Statut == StatutProjet.Terminé),
                        Annule = projets.Count(p => p.Statut == StatutProjet.Annulé)
                    },
                    projetsParMois = GetProjetsParMois(projets),
                    planificationsParJour = GetPlanificationsParJour(planifications),
                    progressionProjets = GetProgressionProjets(projets)
                };

                return Ok(statistiques);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erreur lors de la récupération des statistiques : {ex.Message}");
            }
        }

        private object[] GetProjetsParMois(IEnumerable<Projet> projets)
        {
            // Utiliser DateDebut au lieu de DateCreation
            return projets
                .GroupBy(p => new { p.DateDebut.Year, p.DateDebut.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .Select(g => new
                {
                    mois = $"{g.Key.Month:00}/{g.Key.Year}",
                    count = g.Count()
                })
                .TakeLast(6) // Les 6 derniers mois
                .ToArray();
        }

        private object[] GetPlanificationsParJour(IEnumerable<Planification> planifications)
        {
            var derniersSeptJours = Enumerable.Range(0, 7)
                .Select(i => DateTime.Today.AddDays(-6 + i))
                .ToList();

            return derniersSeptJours.Select(date => new
            {
                jour = date.ToString("dd/MM"),
                count = planifications.Count(p => p.Date.Date == date.Date)
            }).ToArray();
        }

        private object[] GetProgressionProjets(IEnumerable<Projet> projets)
        {
            return projets.Take(5).Select(p => new
            {
                nom = p.Nom,
                progression = CalculerProgressionProjet(p)
            }).ToArray();
        }

        private int CalculerProgressionProjet(Projet projet)
        {
            // Calcul basé sur les dates du projet
            var dateDebut = projet.DateDebut;
            var dateEcheance = projet.DateEcheance;
            var maintenant = DateTime.Now;

            if (maintenant < dateDebut)
            {
                return 0; // Pas encore commencé
            }
            else if (maintenant > dateEcheance)
            {
                return projet.Statut == StatutProjet.Terminé ? 100 : 90; // Terminé ou en retard
            }
            else
            {
                // Calcul basé sur le temps écoulé
                var dureeTotal = (dateEcheance - dateDebut).TotalDays;
                var dureeEcoulee = (maintenant - dateDebut).TotalDays;
                var progression = (int)((dureeEcoulee / dureeTotal) * 100);

                return Math.Max(5, Math.Min(95, progression)); // Entre 5% et 95%
            }
        }
    }
}