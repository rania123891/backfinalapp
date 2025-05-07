// UserService.Domain/Queries/GetUtilisateurQuery.cs
using MediatR;
using UserService.Domain.Models;

namespace UserService.Domain.Queries
{
    public class GetUtilisateurQuery : IRequest<Utilisateur>
    {
        public int Id { get; }

        public GetUtilisateurQuery(int id)
        {
            Id = id;
        }
    }
}
