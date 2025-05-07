using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjetService.Domain.Models;

namespace ProjetService.Domain.DTO
{
    public class AjouterMembreDto
    {
        public int UtilisateurId { get; set; }
        public RoleMembreEquipe Role { get; set; } = RoleMembreEquipe.Membre;
    }
}
