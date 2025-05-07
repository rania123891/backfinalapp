using MediatR;
using System.Collections.Generic;
using UserService.Domain.Models;

namespace UserService.Domain.Queries
{
    public class GetAllUtilisateursQuery : IRequest<IEnumerable<Utilisateur>>
    {
        // Optionnel : Ajoutez des paramètres de filtrage ici si besoin
        // public string RoleFilter { get; set; }
        // public string SearchTerm { get; set; }
    }
}