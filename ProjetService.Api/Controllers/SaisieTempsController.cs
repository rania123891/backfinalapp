using Microsoft.AspNetCore.Mvc;
using MediatR;
using ProjetService.Domain.Commands;
using ProjetService.Domain.DTO;
using ProjetService.Domain.Queries;

namespace ProjetService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SaisieTempsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<SaisieTempsController> _logger;

        public SaisieTempsController(IMediator mediator, ILogger<SaisieTempsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SaisieTempsDto>> GetSaisieTemps(int id)
        {
            try
            {
                var query = new GetSaisieTempsQuery { Id = id };
                var result = await _mediator.Send(query);
                
                if (result == null)
                    return NotFound($"Saisie de temps avec l'ID {id} non trouvée");

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de la saisie de temps {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        [HttpGet("utilisateur/{utilisateurId}")]
        public async Task<ActionResult<List<SaisieTempsDto>>> GetSaisiesTempsParUtilisateur(
            int utilisateurId, 
            [FromQuery] DateTime? dateDebut = null, 
            [FromQuery] DateTime? dateFin = null)
        {
            try
            {
                var query = new GetSaisiesTempsParUtilisateurQuery 
                { 
                    UtilisateurId = utilisateurId,
                    DateDebut = dateDebut,
                    DateFin = dateFin
                };
                
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des saisies de temps pour l'utilisateur {UtilisateurId}", utilisateurId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        [HttpGet("projet/{projetId}")]
        public async Task<ActionResult<List<SaisieTempsDto>>> GetSaisiesTempsParProjet(
            int projetId, 
            [FromQuery] DateTime? dateDebut = null, 
            [FromQuery] DateTime? dateFin = null)
        {
            try
            {
                var query = new GetSaisiesTempsParProjetQuery 
                { 
                    ProjetId = projetId,
                    DateDebut = dateDebut,
                    DateFin = dateFin
                };
                
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des saisies de temps pour le projet {ProjetId}", projetId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        [HttpGet("tache/{tacheId}")]
        public async Task<ActionResult<List<SaisieTempsDto>>> GetSaisiesTempsParTache(
            int tacheId, 
            [FromQuery] DateTime? dateDebut = null, 
            [FromQuery] DateTime? dateFin = null)
        {
            try
            {
                var query = new GetSaisiesTempsParTacheQuery 
                { 
                    TacheId = tacheId,
                    DateDebut = dateDebut,
                    DateFin = dateFin
                };
                
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des saisies de temps pour la tâche {TacheId}", tacheId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        [HttpPost]
        public async Task<ActionResult<int>> CreateSaisieTemps([FromBody] CreateSaisieTempsDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Calculer automatiquement la durée si elle n'est pas fournie
                if (dto.DureeHeures == 0)
                {
                    var duree = dto.HeureFin - dto.HeureDebut;
                    dto.DureeHeures = (decimal)duree.TotalHours;
                }

                var command = new CreateSaisieTempsCommand
                {
                    UtilisateurId = dto.UtilisateurId,
                    ProjetId = dto.ProjetId,
                    TacheId = dto.TacheId,
                    DateTravail = dto.DateTravail,
                    HeureDebut = dto.HeureDebut,
                    HeureFin = dto.HeureFin,
                    DureeHeures = dto.DureeHeures,
                    Description = dto.Description
                };

                var result = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetSaisieTemps), new { id = result }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création de la saisie de temps");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<bool>> UpdateSaisieTemps(int id, [FromBody] UpdateSaisieTempsDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (id != dto.Id)
                    return BadRequest("L'ID dans l'URL ne correspond pas à l'ID dans le corps de la requête");

                // Calculer automatiquement la durée si elle n'est pas fournie
                if (dto.DureeHeures == 0)
                {
                    var duree = dto.HeureFin - dto.HeureDebut;
                    dto.DureeHeures = (decimal)duree.TotalHours;
                }

                var command = new UpdateSaisieTempsCommand
                {
                    Id = dto.Id,
                    TacheId = dto.TacheId,
                    DateTravail = dto.DateTravail,
                    HeureDebut = dto.HeureDebut,
                    HeureFin = dto.HeureFin,
                    DureeHeures = dto.DureeHeures,
                    Description = dto.Description
                };

                var result = await _mediator.Send(command);
                
                if (!result)
                    return NotFound($"Saisie de temps avec l'ID {id} non trouvée");

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour de la saisie de temps {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteSaisieTemps(int id, [FromQuery] int utilisateurId)
        {
            try
            {
                var command = new DeleteSaisieTempsCommand
                {
                    Id = id,
                    UtilisateurId = utilisateurId
                };

                var result = await _mediator.Send(command);
                
                if (!result)
                    return NotFound($"Saisie de temps avec l'ID {id} non trouvée ou non autorisée");

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression de la saisie de temps {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }
    }
} 