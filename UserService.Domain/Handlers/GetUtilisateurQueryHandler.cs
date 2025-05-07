// UserService.Domain/Handlers/GetUtilisateurQueryHandler.cs
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using UserService.Domain.Interfaces;
using UserService.Domain.Models;
using UserService.Domain.Queries;

namespace UserService.Domain.Handlers
{
    public class GetUtilisateurQueryHandler : IRequestHandler<GetUtilisateurQuery, Utilisateur>
    {
        private readonly IUtilisateurRepository _repository;

        public GetUtilisateurQueryHandler(IUtilisateurRepository repository)
        {
            _repository = repository;
        }

        public Task<Utilisateur> Handle(GetUtilisateurQuery request, CancellationToken cancellationToken)
        {
            var utilisateur = _repository.GetById(request.Id);
            return Task.FromResult(utilisateur);
        }
    }
}
