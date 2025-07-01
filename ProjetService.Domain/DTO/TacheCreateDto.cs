using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjetService.Domain.Models;

namespace ProjetService.Domain.DTO
{
    public class TacheCreateDto
    {
        [Required]
        public string Titre { get; set; }

        [Required]
        public PrioriteTache Priorite { get; set; }

        [Required]
        public int EquipeId { get; set; }
    }

}
