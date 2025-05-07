using System.Threading;
using System.Threading.Tasks;
using MediatR;
using UserService.Domain.Interfaces;
using UserService.Domain.Models;
using UserService.Domain.Queries;

namespace UserService.Domain.Handlers
{
    public class GetUtilisateurByEmailQueryHandler : IRequestHandler<GetUtilisateurByEmailQuery, Utilisateur>
    {
        private readonly IUtilisateurRepository _repository;

        public GetUtilisateurByEmailQueryHandler(IUtilisateurRepository repository)
        {
            _repository = repository;
        }

        public Task<Utilisateur> Handle(GetUtilisateurByEmailQuery request, CancellationToken cancellationToken)
        {
            var utilisateur = _repository.GetByEmail(request.Email);
            return Task.FromResult(utilisateur);
        }
    }
}
