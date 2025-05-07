// UserService.Domain/Commands/CreateUtilisateurCommand.cs
using MediatR;
using UserService.Domain.Models;

namespace UserService.Domain.Commands
{
    public class CreateUtilisateurCommand : IRequest<Utilisateur>
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } = "User"; // Valeur par défaut "User"
        public string Nom { get; set; } // Nouveau champ
        public string Prenom { get; set; } // Nouveau champ
        public CreateUtilisateurCommand(string email, string password, string role, string nom, string prenom)
        {
            Email = email;
            Password = password;
            Role = role;
            Nom = nom;
            Prenom = prenom;
        }
    }
}
