using MediatR;
using ProjetService.Domain.DTO;

namespace ProjetService.Domain.Queries
{
    public class GetSaisieTempsQuery : IRequest<SaisieTempsDto>
    {
        public int Id { get; set; }
    }

    public class GetSaisiesTempsParUtilisateurQuery : IRequest<List<SaisieTempsDto>>
    {
        public int UtilisateurId { get; set; }
        public DateTime? DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
    }

    public class GetSaisiesTempsParProjetQuery : IRequest<List<SaisieTempsDto>>
    {
        public int ProjetId { get; set; }
        public DateTime? DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
    }

    public class GetSaisiesTempsParTacheQuery : IRequest<List<SaisieTempsDto>>
    {
        public int TacheId { get; set; }
        public DateTime? DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
    }
} 