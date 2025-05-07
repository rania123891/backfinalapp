using Microsoft.AspNetCore.Mvc;
using ProjetService.Domain.Models;
using ProjetService.Domain.Interfaces;
using MediatR;

namespace ProjetService.Api.Controllers
{
    [Route("api/projets")]
    [ApiController]
    public class ProjetController : GenericController<Projet>
    {
        public ProjetController(IMediator mediator, IGenericRepository<Projet> repository)
            : base(mediator, repository) // ✅ Passer IMediator au contrôleur parent
        {
        }
    }
}
