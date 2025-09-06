using MediatR;
using ProjetService.Domain.Commands;
using ProjetService.Domain.DTO;
using ProjetService.Domain.Interface;
using ProjetService.Domain.Interfaces;
using ProjetService.Domain.Models;
using ProjetService.Domain.Queries;

namespace ProjetService.Domain.Handlers
{
    // Handlers pour les commandes
    public class CreateSaisieTempsHandler : IRequestHandler<CreateSaisieTempsCommand, int>
    {
        private readonly ISaisieTempsRepository _saisieTempsRepository;

        public CreateSaisieTempsHandler(ISaisieTempsRepository saisieTempsRepository)
        {
            _saisieTempsRepository = saisieTempsRepository;
        }

        public async Task<int> Handle(CreateSaisieTempsCommand request, CancellationToken cancellationToken)
        {
            var saisieTemps = new SaisieTemps
            {
                UtilisateurId = request.UtilisateurId,
                ProjetId = request.ProjetId,
                TacheId = request.TacheId,
                DateTravail = request.DateTravail,
                HeureDebut = request.HeureDebut,
                HeureFin = request.HeureFin,
                DureeHeures = request.DureeHeures,
                Description = request.Description,
                CreatedAt = DateTime.UtcNow
            };

            return await _saisieTempsRepository.CreateAsync(saisieTemps);
        }
    }

    public class UpdateSaisieTempsHandler : IRequestHandler<UpdateSaisieTempsCommand, bool>
    {
        private readonly ISaisieTempsRepository _saisieTempsRepository;

        public UpdateSaisieTempsHandler(ISaisieTempsRepository saisieTempsRepository)
        {
            _saisieTempsRepository = saisieTempsRepository;
        }

        public async Task<bool> Handle(UpdateSaisieTempsCommand request, CancellationToken cancellationToken)
        {
            var saisieTemps = await _saisieTempsRepository.GetByIdAsync(request.Id);
            if (saisieTemps == null)
                return false;

            saisieTemps.TacheId = request.TacheId;
            saisieTemps.DateTravail = request.DateTravail;
            saisieTemps.HeureDebut = request.HeureDebut;
            saisieTemps.HeureFin = request.HeureFin;
            saisieTemps.DureeHeures = request.DureeHeures;
            saisieTemps.Description = request.Description;

            return await _saisieTempsRepository.UpdateAsync(saisieTemps);
        }
    }

    public class DeleteSaisieTempsHandler : IRequestHandler<DeleteSaisieTempsCommand, bool>
    {
        private readonly ISaisieTempsRepository _saisieTempsRepository;

        public DeleteSaisieTempsHandler(ISaisieTempsRepository saisieTempsRepository)
        {
            _saisieTempsRepository = saisieTempsRepository;
        }

        public async Task<bool> Handle(DeleteSaisieTempsCommand request, CancellationToken cancellationToken)
        {
            var saisieTemps = await _saisieTempsRepository.GetByIdAsync(request.Id);
            if (saisieTemps == null || saisieTemps.UtilisateurId != request.UtilisateurId)
                return false;

            return await _saisieTempsRepository.DeleteAsync(request.Id);
        }
    }

    // Handlers pour les queries
    public class GetSaisieTempsHandler : IRequestHandler<GetSaisieTempsQuery, SaisieTempsDto>
    {
        private readonly ISaisieTempsRepository _saisieTempsRepository;

        public GetSaisieTempsHandler(ISaisieTempsRepository saisieTempsRepository)
        {
            _saisieTempsRepository = saisieTempsRepository;
        }

        public async Task<SaisieTempsDto> Handle(GetSaisieTempsQuery request, CancellationToken cancellationToken)
        {
            var saisieTemps = await _saisieTempsRepository.GetByIdAsync(request.Id);
            if (saisieTemps == null)
                return null;

            return new SaisieTempsDto
            {
                Id = saisieTemps.Id,
                UtilisateurId = saisieTemps.UtilisateurId,
                ProjetId = saisieTemps.ProjetId,
                TacheId = saisieTemps.TacheId,
                DateTravail = saisieTemps.DateTravail,
                HeureDebut = saisieTemps.HeureDebut,
                HeureFin = saisieTemps.HeureFin,
                DureeHeures = saisieTemps.DureeHeures,
                Description = saisieTemps.Description,
                CreatedAt = saisieTemps.CreatedAt
            };
        }
    }

    public class GetSaisiesTempsParUtilisateurHandler : IRequestHandler<GetSaisiesTempsParUtilisateurQuery, List<SaisieTempsDto>>
    {
        private readonly ISaisieTempsRepository _saisieTempsRepository;

        public GetSaisiesTempsParUtilisateurHandler(ISaisieTempsRepository saisieTempsRepository)
        {
            _saisieTempsRepository = saisieTempsRepository;
        }

        public async Task<List<SaisieTempsDto>> Handle(GetSaisiesTempsParUtilisateurQuery request, CancellationToken cancellationToken)
        {
            var saisiesTemps = await _saisieTempsRepository.GetByUtilisateurIdAsync(request.UtilisateurId, request.DateDebut, request.DateFin);
            
            return saisiesTemps.Select(s => new SaisieTempsDto
            {
                Id = s.Id,
                UtilisateurId = s.UtilisateurId,
                ProjetId = s.ProjetId,
                TacheId = s.TacheId,
                DateTravail = s.DateTravail,
                HeureDebut = s.HeureDebut,
                HeureFin = s.HeureFin,
                DureeHeures = s.DureeHeures,
                Description = s.Description,
                CreatedAt = s.CreatedAt
            }).ToList();
        }
    }

    public class GetSaisiesTempsParProjetHandler : IRequestHandler<GetSaisiesTempsParProjetQuery, List<SaisieTempsDto>>
    {
        private readonly ISaisieTempsRepository _saisieTempsRepository;

        public GetSaisiesTempsParProjetHandler(ISaisieTempsRepository saisieTempsRepository)
        {
            _saisieTempsRepository = saisieTempsRepository;
        }

        public async Task<List<SaisieTempsDto>> Handle(GetSaisiesTempsParProjetQuery request, CancellationToken cancellationToken)
        {
            var saisiesTemps = await _saisieTempsRepository.GetByProjetIdAsync(request.ProjetId, request.DateDebut, request.DateFin);
            
            return saisiesTemps.Select(s => new SaisieTempsDto
            {
                Id = s.Id,
                UtilisateurId = s.UtilisateurId,
                ProjetId = s.ProjetId,
                TacheId = s.TacheId,
                DateTravail = s.DateTravail,
                HeureDebut = s.HeureDebut,
                HeureFin = s.HeureFin,
                DureeHeures = s.DureeHeures,
                Description = s.Description,
                CreatedAt = s.CreatedAt
            }).ToList();
        }
    }

    public class GetSaisiesTempsParTacheHandler : IRequestHandler<GetSaisiesTempsParTacheQuery, List<SaisieTempsDto>>
    {
        private readonly ISaisieTempsRepository _saisieTempsRepository;

        public GetSaisiesTempsParTacheHandler(ISaisieTempsRepository saisieTempsRepository)
        {
            _saisieTempsRepository = saisieTempsRepository;
        }

        public async Task<List<SaisieTempsDto>> Handle(GetSaisiesTempsParTacheQuery request, CancellationToken cancellationToken)
        {
            var saisiesTemps = await _saisieTempsRepository.GetByTacheIdAsync(request.TacheId, request.DateDebut, request.DateFin);
            
            return saisiesTemps.Select(s => new SaisieTempsDto
            {
                Id = s.Id,
                UtilisateurId = s.UtilisateurId,
                ProjetId = s.ProjetId,
                TacheId = s.TacheId,
                DateTravail = s.DateTravail,
                HeureDebut = s.HeureDebut,
                HeureFin = s.HeureFin,
                DureeHeures = s.DureeHeures,
                Description = s.Description,
                CreatedAt = s.CreatedAt
            }).ToList();
        }
    }

    public class GetFicheHeureHandler : IRequestHandler<GetFicheHeureQuery, FicheHeureResult>
    {
        private readonly ISaisieTempsRepository _saisieTempsRepository;
        private readonly IGenericRepository<Projet> _genericRepository;
        private readonly IPlanificationRepository _planificationRepository; // AJOUT

        public GetFicheHeureHandler(
            ISaisieTempsRepository saisieTempsRepository,
            IGenericRepository<Projet> genericRepository,
            IPlanificationRepository planificationRepository) // AJOUT
        {
            _saisieTempsRepository = saisieTempsRepository;
            _genericRepository = genericRepository;
            _planificationRepository = planificationRepository; // AJOUT
        }

        public async Task<FicheHeureResult> Handle(GetFicheHeureQuery request, CancellationToken cancellationToken)
        {
            // RÉCUPÉRER LES PLANIFICATIONS au lieu des SaisieTemps
            var planifications = await _planificationRepository.GetByProjetIdWithIncludesAsync(
                request.ProjetId,
                request.DateDebut,
                request.DateFin);

            var projet = await _genericRepository.GetByIdAsync(request.ProjetId);

            // Filtrer par utilisateur si spécifié
            if (request.UtilisateurId.HasValue)
            {
                planifications = planifications.Where(p => p.UserId == request.UtilisateurId.Value);
            }

            // CONVERTIR les Planifications en SaisieTempsDto
            // CONVERTIR les Planifications en SaisieTempsDto
            var saisiesTempsDto = planifications.Select(p => new SaisieTempsDto
            {
                Id = p.Id,
                UtilisateurId = p.UserId,
                ProjetId = p.ProjetId,
                TacheId = p.TacheId,
                DateTravail = p.Date,
                HeureDebut = p.HeureDebut,                               // ← DIRECT TimeSpan
                HeureFin = p.HeureFin,                                   // ← DIRECT TimeSpan
                DureeHeures = (decimal)CalculerDureeHeures(p.HeureDebut, p.HeureFin), // ← CAST en decimal
                Description = p.Description,
                CreatedAt = DateTime.UtcNow,
                NomProjet = p.Projet?.Nom,
                NomTache = p.Tache?.Titre,
                NomUtilisateur = $"Utilisateur_{p.UserId}"
            }).ToList();

            // CALCULER les totaux depuis les planifications
            var totalHeures = saisiesTempsDto.Sum(s => s.DureeHeures);

            var heuresParUtilisateur = saisiesTempsDto
                .GroupBy(s => s.UtilisateurId.ToString())
                .ToDictionary(g => g.Key, g => g.Sum(s => s.DureeHeures));

            var heuresParTache = saisiesTempsDto
                .Where(s => s.TacheId.HasValue)
                .GroupBy(s => s.TacheId.Value.ToString())
                .ToDictionary(g => g.Key, g => g.Sum(s => s.DureeHeures));

            return new FicheHeureResult
            {
                ProjetId = request.ProjetId,
                NomProjet = projet?.Nom ?? "Projet non trouvé",
                DateDebut = request.DateDebut,
                DateFin = request.DateFin,
                SaisiesTemps = saisiesTempsDto, // Maintenant avec les vraies données !
                TotalHeures = totalHeures,
                HeuresParUtilisateur = heuresParUtilisateur,
                HeuresParTache = heuresParTache
            };
        }

        private static decimal CalculerDureeHeures(TimeSpan heureDebut, TimeSpan heureFin)
        {
            return (decimal)(heureFin - heureDebut).TotalHours;
        }
    }
}
