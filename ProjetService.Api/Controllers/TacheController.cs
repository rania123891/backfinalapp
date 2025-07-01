using Microsoft.AspNetCore.Mvc;
using ProjetService.Domain.Models;
using ProjetService.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProjetService.Data.Context;
using ProjetService.Domain.DTO;

namespace ProjetService.Api.Controllers
{
    [Route("api/taches")]
    [ApiController]
    public class TacheController : GenericController<Tache>
    {
        private readonly ProjetDbContext _context;

        public TacheController(IMediator mediator, IGenericRepository<Tache> repository, ProjetDbContext context)
            : base(mediator, repository)
        {
            _context = context;
        }

        [HttpGet("gantt")]
        public async Task<ActionResult<List<TacheGanttDto>>> GetTachesPourGantt([FromQuery] int? projetId = null)
        {
            try
            {
                IQueryable<Planification> query = _context.Planifications
                    .Include(p => p.Tache)
                    .Include(p => p.Projet);

                if (projetId.HasValue)
                {
                    query = query.Where(p => p.ProjetId == projetId.Value);
                }

                var planifications = await query.ToListAsync();

                if (!planifications.Any())
                {
                    return Ok(new List<TacheGanttDto>());
                }

                var tachesGantt = planifications
                    .GroupBy(p => new { p.TacheId, p.Tache.Titre })
                    .Select(group => {
                        var plans = group.OrderBy(p => p.Date).ThenBy(p => p.HeureDebut).ToList();
                        var startDateTime = plans.First().Date.Add(plans.First().HeureDebut);
                        var endDateTime = plans.Last().Date.Add(plans.Last().HeureFin);

                        return new TacheGanttDto
                        {
                            Id = group.Key.TacheId.ToString(),
                            Name = group.Key.Titre,
                            Start = startDateTime,
                            End = endDateTime,
                            Dependencies = ""
                        };
                    })
                    .OrderBy(t => t.Start)
                    .ToList();

                return Ok(tachesGantt);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la récupération des données: {ex.Message}");
            }
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllTachesWithEquipes()
        {
            var taches = await _context.Taches
                .Include(t => t.Equipe)
                .ToListAsync();
            return Ok(taches);
        }

        [HttpGet("equipe/{equipeId}")]
        public async Task<IActionResult> GetTachesByEquipe(int equipeId)
        {
            var taches = await _context.Taches
                .Where(t => t.EquipeId == equipeId)
                .Include(t => t.Equipe)
                .ToListAsync();
            return Ok(taches);
        }


        [HttpPost("create")]
        public async Task<IActionResult> CreateTache([FromBody] TacheCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var tache = new Tache
            {
                Titre = dto.Titre,
                Priorite = dto.Priorite,
                EquipeId = dto.EquipeId
            };

            _context.Taches.Add(tache);
            await _context.SaveChangesAsync();
            return Ok(tache);
        }

    }
}