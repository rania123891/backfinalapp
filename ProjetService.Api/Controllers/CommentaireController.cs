using Microsoft.AspNetCore.Mvc;
using ProjetService.Domain.Models;
using ProjetService.Domain.Interfaces;
using MediatR;

namespace ProjetService.Api.Controllers
{
    [Route("api/commentaires")]
    [ApiController]
    public class CommentaireController : GenericController<Commentaire>
    {
        public CommentaireController(IMediator mediator, IGenericRepository<Commentaire> repository)
            : base(mediator, repository)
        {
        }
    }
}
