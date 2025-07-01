using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetService.Domain.Models;
using ProjetService.Domain.Interfaces;
using MediatR;
using ProjetService.Data.Context;

namespace ProjetService.Api.Controllers
{
    [Route("api/projets")]
    [ApiController]
    public class ProjetController : GenericController<Projet>
    {
        private readonly ProjetDbContext _context;

        public ProjetController(IMediator mediator, IGenericRepository<Projet> repository, ProjetDbContext context)
            : base(mediator, repository)
        {
            _context = context;
        }

        [HttpGet("{id}/equipes")]
        public async Task<ActionResult<IEnumerable<Equipe>>> GetEquipesDuProjet(int id)
        {
            var projet = await _context.Projets
                .Include(p => p.ProjetsEquipes)
                    .ThenInclude(pe => pe.Equipe)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (projet == null)
                return NotFound();

            var equipes = projet.ProjetsEquipes.Select(pe => pe.Equipe).ToList();
            return Ok(equipes);
        }
    }
}