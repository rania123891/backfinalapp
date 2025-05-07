using Microsoft.AspNetCore.Mvc;
using ProjetService.Domain.Models;
using ProjetService.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProjetService.Data.Context;

namespace ProjetService.Api.Controllers
{
    [Route("api/tableaux")]
    [ApiController]
    public class TableauController : GenericController<Tableau>
    {
        private readonly ProjetDbContext _context;

        public TableauController(IMediator mediator, IGenericRepository<Tableau> repository, ProjetDbContext context)
            : base(mediator, repository)
        {
            _context = context;
        }

        // GET: api/tableaux/projet/5
        [HttpGet("projet/{projetId}")]
        public async Task<ActionResult> GetTableauxByProjet(int projetId)
        {
            var tableaux = await _context.Tableaux
                .Where(t => t.ProjetId == projetId)
                .ToListAsync();

            return Ok(tableaux);
        }

        // GET: api/tableaux/5/listes
        [HttpGet("{tableauId}/listes")]
        public async Task<ActionResult> GetListesByTableau(int tableauId)
        {
            var listes = await _context.Listes
                .Where(l => l.TableauId == tableauId)
                .ToListAsync();

            return Ok(listes);
        }
    }
}