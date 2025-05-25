using Microsoft.AspNetCore.Mvc;
using ProjetService.Infrastructure.Services;
using ProjetService.Domain.Interfaces;
using ProjetService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjetService.Api.Controllers
{
    [Route("api/eta")]
    [ApiController]
    public class ETAController : ControllerBase
    {
        private readonly IETAPredictionService _etaService;
        private readonly IGenericRepository<Tache> _tacheRepository;
        private readonly IGenericRepository<Projet> _projetRepository;

        public ETAController(
            IETAPredictionService etaService,
            IGenericRepository<Tache> tacheRepository,
            IGenericRepository<Projet> projetRepository)
        {
            _etaService = etaService;
            _tacheRepository = tacheRepository;
            _projetRepository = projetRepository;
        }

        /// <summary>
        /// 🎯 Prédire la durée estimée d'une tâche
        /// </summary>
        [HttpPost("predict")]
        public async Task<ActionResult<ETAResult>> PredireDureeTache([FromBody] PredictionRequest request)
        {
            try
            {
                var tache = await _tacheRepository.GetByIdAsync(request.TacheId);
                if (tache == null)
                    return NotFound($"Tâche {request.TacheId} introuvable");

                var projet = await _projetRepository.GetByIdAsync(request.ProjetId);
                if (projet == null)
                    return NotFound($"Projet {request.ProjetId} introuvable");

                var prediction = await _etaService.PredireDureeTacheAsync(tache, projet);
                return Ok(prediction);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erreur lors de la prédiction : {ex.Message}");
            }
        }

        /// <summary>
        /// 🎯 Prédire pour une nouvelle tâche (pas encore en base)
        /// </summary>
        [HttpPost("predict-new")]
        public async Task<ActionResult<ETAResult>> PredireNouvelleTache([FromBody] NouvelleTacheRequest request)
        {
            try
            {
                var tache = new Tache
                {
                    Titre = request.Titre,
                    Priorite = request.Priorite
                };

                var projet = await _projetRepository.GetByIdAsync(request.ProjetId);
                if (projet == null)
                    return NotFound($"Projet {request.ProjetId} introuvable");

                var prediction = await _etaService.PredireDureeTacheAsync(tache, projet);
                return Ok(prediction);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erreur lors de la prédiction : {ex.Message}");
            }
        }

        /// <summary>
        /// 🎓 Entraîner le modèle avec les données actuelles
        /// </summary>
        [HttpPost("train")]
        public async Task<ActionResult<object>> EntrainerModele()
        {
            try
            {
                var succes = await _etaService.EntrainerModeleAsync();

                if (succes)
                {
                    var stats = await _etaService.GetStatistiquesModeleAsync();
                    return Ok(new
                    {
                        Success = true,
                        Message = "🎓 Modèle entraîné avec succès!",
                        Stats = stats
                    });
                }
                else
                {
                    return BadRequest("❌ Échec de l'entraînement du modèle");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Erreur lors de l'entraînement : {ex.Message}");
            }
        }

        /// <summary>
        /// 📊 Obtenir les statistiques du modèle
        /// </summary>
        [HttpGet("stats")]
        public async Task<ActionResult<object>> GetStatistiques()
        {
            try
            {
                var stats = await _etaService.GetStatistiquesModeleAsync();
                return Ok(new { Stats = stats });
            }
            catch (Exception ex)
            {
                return BadRequest($"Erreur : {ex.Message}");
            }
        }

        /// <summary>
        /// 🔄 Prédictions en lot pour plusieurs tâches
        /// </summary>
        [HttpPost("predict-batch")]
        public async Task<ActionResult<List<ETAResultWithId>>> PredirePlusieurs([FromBody] List<PredictionRequest> requests)
        {
            try
            {
                var results = new List<ETAResultWithId>();

                foreach (var request in requests)
                {
                    var tache = await _tacheRepository.GetByIdAsync(request.TacheId);
                    var projet = await _projetRepository.GetByIdAsync(request.ProjetId);

                    if (tache != null && projet != null)
                    {
                        var prediction = await _etaService.PredireDureeTacheAsync(tache, projet);
                        results.Add(new ETAResultWithId
                        {
                            TacheId = request.TacheId,
                            ProjetId = request.ProjetId,
                            Prediction = prediction
                        });
                    }
                }

                return Ok(results);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erreur lors des prédictions : {ex.Message}");
            }
        }
    }

    // 📋 DTOs pour les requêtes
    public class PredictionRequest
    {
        public int TacheId { get; set; }
        public int ProjetId { get; set; }
    }

    public class NouvelleTacheRequest
    {
        public string Titre { get; set; }
        public PrioriteTache Priorite { get; set; }
        public int ProjetId { get; set; }
    }

    public class ETAResultWithId
    {
        public int TacheId { get; set; }
        public int ProjetId { get; set; }
        public ETAResult Prediction { get; set; }
    }
}