using Microsoft.AspNetCore.Mvc;
using ProjetService.Domain.Interfaces;
using ProjetService.Domain.Models;
using ProjetService.Domain.Dto;
using ProjetService.Domain.Interface;

namespace ProjetService.Api.Controllers
{
    [Route("api/planifications")]
    [ApiController]
    public class PlanificationController : ControllerBase
    {
        private readonly IPlanificationRepository _planificationRepository;

        public PlanificationController(IPlanificationRepository planificationRepository)
        {
            _planificationRepository = planificationRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Planification>>> GetPlanifications()
        {
            var planifications = await _planificationRepository.GetAllWithIncludesAsync();
            return Ok(planifications);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Planification>> GetPlanification(int id)
        {
            var planification = await _planificationRepository.GetByIdWithIncludesAsync(id);
            if (planification == null)
                return NotFound();

            return Ok(planification);
        }

        [HttpGet("date/{date}")]
        public async Task<ActionResult<IEnumerable<Planification>>> GetPlanificationsByDate(DateTime date)
        {
            var planifications = await _planificationRepository.GetByDateAsync(date);
            return Ok(planifications);
        }

        [HttpPost]
        public async Task<ActionResult<Planification>> CreatePlanification([FromBody] CreatePlanificationDto dto)
        {
            try
            {
                if (!TimeSpan.TryParse(dto.HeureDebut, out var heureDebut))
                    return BadRequest("Format d'heure de début invalide");

                if (!TimeSpan.TryParse(dto.HeureFin, out var heureFin))
                    return BadRequest("Format d'heure de fin invalide");

                if (heureFin <= heureDebut)
                    return BadRequest("L'heure de fin doit être postérieure à l'heure de début");

                var planification = new Planification
                {
                    Date = dto.Date.Date,
                    HeureDebut = heureDebut,
                    HeureFin = heureFin,
                    Description = dto.Description,
                    TacheId = dto.TacheId,
                    ProjetId = dto.ProjetId,
                    ListeId = dto.ListeId
                };

                var created = await _planificationRepository.CreateWithIncludesAsync(planification);
                return CreatedAtAction(nameof(GetPlanification), new { id = created.Id }, created);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erreur : {ex.Message}");
            }
        }

        [HttpPatch("{id}/statut")]
        public async Task<ActionResult<Planification>> UpdatePlanificationStatus(int id, [FromBody] UpdatePlanificationStatusDto dto)
        {
            var planification = await _planificationRepository.UpdateStatusAsync(id, dto.ListeId);

            if (planification == null)
                return NotFound();

            return Ok(planification);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePlanification(int id)
        {
            var planification = await _planificationRepository.GetByIdAsync(id);
            if (planification == null)
                return NotFound();

            await _planificationRepository.DeleteAsync(planification);
            return NoContent();
        }
    }
}