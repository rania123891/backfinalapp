using MediatR;
using UserService.Domain.Commands;
using UserService.Domain.Interfaces;
using UserService.Domain.Models;

namespace UserService.Domain.Handlers
{
    public class CreateUtilisateurCommandHandler : IRequestHandler<CreateUtilisateurCommand, Utilisateur>
    {
        private readonly IUtilisateurRepository _repository;
        private readonly IAuthService _authService;

        public CreateUtilisateurCommandHandler(IUtilisateurRepository repository, IAuthService authService)
        {
            _repository = repository;
            _authService = authService;
        }

        public Task<Utilisateur> Handle(CreateUtilisateurCommand request, CancellationToken cancellationToken)
        {
            // Vérification si l'utilisateur existe déjà
            if (_repository.GetByEmail(request.Email) != null)
            {
                throw new Exception("Un utilisateur avec cet email existe déjà.");
            }

            // Hacher le mot de passe
            string passwordHash = _authService.HashPassword(request.Password);

            // Déterminer le rôle (vérifier si le rôle est valide)
            if (!Enum.TryParse(request.Role, true, out RoleUtilisateur role))
            {
                role = RoleUtilisateur.User; // Valeur par défaut
            }

            // Créer l'utilisateur avec le rôle fourni
            var utilisateur = new Utilisateur
            {
                Email = request.Email,
                PasswordHash = passwordHash,
                Role = role, // ✅ Assigner le rôle
                Nom = request.Nom,
                Prenom = request.Prenom,
                DateCreation = DateTime.UtcNow
            };

            _repository.Add(utilisateur);

            return Task.FromResult(utilisateur);
        }
    }
}
