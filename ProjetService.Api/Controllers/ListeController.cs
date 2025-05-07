using Microsoft.AspNetCore.Mvc;
using ProjetService.Domain.Models;
using ProjetService.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProjetService.Data.Context;

namespace ProjetService.Api.Controllers
{
    [Route("api/listes")]
    [ApiController]
    public class ListeController : GenericController<Liste>
    {
        private readonly ProjetDbContext _context;

        public ListeController(IMediator mediator, IGenericRepository<Liste> repository, ProjetDbContext context)
            : base(mediator, repository)
        {
            _context = context;
        }

        [HttpGet("tableau/{tableauId}")]
        public async Task<ActionResult> GetListesByTableau(int tableauId)
        {
            return Ok(await _context.Listes
                .Where(l => l.TableauId == tableauId)
                .ToListAsync());
        }
    }
}