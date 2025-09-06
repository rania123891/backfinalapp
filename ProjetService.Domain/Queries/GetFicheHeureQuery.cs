using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using ProjetService.Domain.DTO;

namespace ProjetService.Domain.Queries
{
    public class GetFicheHeureQuery : IRequest<FicheHeureResult>
    {
        public int ProjetId { get; set; }
        public int? UtilisateurId { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }
    }
}
