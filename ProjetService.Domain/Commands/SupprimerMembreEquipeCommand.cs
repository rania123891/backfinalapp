using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace ProjetService.Domain.Commands
{
    // ProjetService.Domain/Commands/EquipeMembre/SupprimerMembreEquipeCommand.cs
    public class SupprimerMembreEquipeCommand : IRequest<bool>
    {
        public int EquipeId { get; set; }
        public int UtilisateurId { get; set; }

        public SupprimerMembreEquipeCommand(int equipeId, int utilisateurId)
        {
            EquipeId = equipeId;
            UtilisateurId = utilisateurId;
        }
    }
}
