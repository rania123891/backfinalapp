using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjetService.Domain.Commands;
using ProjetService.Domain.Models;
using ProjetService.Domain.Interfaces;

namespace ProjetService.Domain.Handlers
{
    public class CreateProjetCommandHandler : IRequestHandler<CreateProjetCommand, Projet>
    {
        private readonly IGenericRepository<Projet> _repository;

        public CreateProjetCommandHandler(IGenericRepository<Projet> repository)
        {
            _repository = repository;
        }

        public async Task<Projet> Handle(CreateProjetCommand request, CancellationToken cancellationToken)
        {
            var projet = new Projet
            {
                Nom = request.Nom,
                Description = request.Description ?? "Créé via assistant vocal",
                DateDebut = request.DateDebut,
                DateEcheance = request.DateEcheance,
                Statut = request.Statut,
                CreateurId = request.CreateurId,
                Duree = request.Duree ?? 0
            };

            await _repository.AddAsync(projet);
            return projet;
        }
    }

    public class UpdateProjetCommandHandler : IRequestHandler<UpdateProjetCommand, Projet>
    {
        private readonly IGenericRepository<Projet> _repository;

        public UpdateProjetCommandHandler(IGenericRepository<Projet> repository)
        {
            _repository = repository;
        }

        public async Task<Projet> Handle(UpdateProjetCommand request, CancellationToken cancellationToken)
        {
            var projet = await _repository.GetByIdAsync(request.Id);
            if (projet == null) throw new Exception("Projet non trouvé");

            projet.Nom = request.Nom;
            projet.Description = request.Description;
            projet.DateDebut = request.DateDebut;
            projet.DateEcheance = request.DateEcheance;
            projet.Statut = request.Statut;

            await _repository.UpdateAsync(projet);
            return projet;
        }
    }

    public class DeleteProjetCommandHandler : IRequestHandler<DeleteProjetCommand, bool>
    {
        private readonly IGenericRepository<Projet> _repository;

        public DeleteProjetCommandHandler(IGenericRepository<Projet> repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(DeleteProjetCommand request, CancellationToken cancellationToken)
        {
            await _repository.DeleteByIdAsync(request.Id);
            return true;
        }
    }
}