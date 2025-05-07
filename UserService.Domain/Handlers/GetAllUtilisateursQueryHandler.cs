using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UserService.Domain.Models;
using UserService.Domain.Interfaces;

namespace UserService.Domain.Queries
{
    public class GetAllUtilisateursQueryHandler : IRequestHandler<GetAllUtilisateursQuery, IEnumerable<Utilisateur>>
    {
        private readonly IUtilisateurRepository _repository;

        public GetAllUtilisateursQueryHandler(IUtilisateurRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Utilisateur>> Handle(
            GetAllUtilisateursQuery request,
            CancellationToken cancellationToken)
        {
            return await _repository.GetAllAsync();
        }
    }
}