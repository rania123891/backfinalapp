// CreateTacheCommandHandler.cs
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjetService.Domain.Commands;
using ProjetService.Domain.Models;
using ProjetService.Domain.Interfaces;

namespace ProjetService.Domain.Handlers
{
    public class CreateTacheCommandHandler : IRequestHandler<CreateTacheCommand, Tache>
    {
        private readonly IGenericRepository<Tache> _tacheRepository;
        private readonly IGenericRepository<Equipe> _equipeRepository;

        public CreateTacheCommandHandler(
            IGenericRepository<Tache> tacheRepository,
            IGenericRepository<Equipe> equipeRepository)
        {
            _tacheRepository = tacheRepository;
            _equipeRepository = equipeRepository;
        }

        public async Task<Tache> Handle(CreateTacheCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // ✅ Vérifier que l'équipe existe
                var equipe = await _equipeRepository.GetByIdAsync(request.EquipeId);
                if (equipe == null)
                {
                    throw new ArgumentException($"Équipe avec l'ID {request.EquipeId} non trouvée");
                }

                var tache = new Tache
                {
                    Titre = request.Titre.Trim(),  // ✅ Titre au lieu de Nom
                    Priorite = request.Priorite,   // ✅ Enum
                    EquipeId = request.EquipeId,   // ✅ ID de l'équipe
                    Commentaires = new List<Commentaire>(),
                    Planifications = new List<Planification>()
                };

                await _tacheRepository.AddAsync(tache);
                return tache;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la création de la tâche: {ex.Message}", ex);
            }
        }
    }
}