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

        [HttpGet("liste/{listeId}")]
        public async Task<ActionResult> GetTachesByListe(int listeId)
        {
            return Ok(await _context.Taches
                .Where(t => t.ListeId == listeId)
                .ToListAsync());
        }
        [HttpGet("gantt")]
        public async Task<ActionResult<List<TacheGanttDto>>> GetTachesPourGantt()
        {
            var taches = await _context.Taches.ToListAsync();

            var tachesGantt = taches.Select(t => new TacheGanttDto
            {
                Id = t.Id.ToString(),
                Name = t.Titre,
                Start = t.DateCreation,
                End = t.DateEcheance,
                Dependencies = "" // pour l'instant pas de dépendances
            }).ToList();

            return Ok(tachesGantt);
        }
    }
}