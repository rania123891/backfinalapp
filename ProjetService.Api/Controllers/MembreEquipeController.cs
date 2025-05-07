using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProjetService.Domain.Interfaces;
using ProjetService.Domain.Models;

namespace ProjetService.Api.Controllers
{
    [Route("api/membres")]
    [ApiController]
    public class MembreEquipeController : GenericController<MembreEquipe>
    {
        public MembreEquipeController(IMediator mediator, IGenericRepository<MembreEquipe> repository)
            : base(mediator, repository) // ✅ Passer IMediator au contrôleur parent
        {
        }
    }
}