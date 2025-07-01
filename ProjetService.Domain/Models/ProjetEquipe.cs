using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetService.Domain.Models
{
    namespace ProjetService.Domain.Models
    {
        public class ProjetEquipe
        {
            public int ProjetId { get; set; }
            public Projet Projet { get; set; }

            public int EquipeId { get; set; }
            public Equipe Equipe { get; set; }
        }
    }

}
