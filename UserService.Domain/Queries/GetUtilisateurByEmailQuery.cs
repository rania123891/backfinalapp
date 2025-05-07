using MediatR;
using UserService.Domain.Models;

namespace UserService.Domain.Queries
{
    public class GetUtilisateurByEmailQuery : IRequest<Utilisateur>
    {
        public string Email { get; }

        public GetUtilisateurByEmailQuery(string email)
        {
            Email = email;
        }
    }
}
