using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace ProjetService.Domain.Commands
{
    public class AjouterMembreEquipeCommand : IRequest<EquipeUtilisateur>
    {
        public int EquipeId { get; set; }
        public int UtilisateurId { get; set; }
        public string Role { get; set; }

        public AjouterMembreEquipeCommand(int equipeId, int utilisateurId, string role)
        {
            EquipeId = equipeId;
            UtilisateurId = utilisateurId;
            Role = role;
        }
    }

}
