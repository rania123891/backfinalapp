using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetService.Domain.Models
{
    public enum PrioriteTache
    {
        Faible,
        Moyenne,
        Elevee
    }

    public class Tache
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Titre { get; set; }


        [Required]
        public PrioriteTache Priorite { get; set; }


     
       
        public ICollection<Commentaire>? Commentaires { get; set; } = new List<Commentaire>();
        public virtual ICollection<Planification> Planifications { get; set; }


    }

}
