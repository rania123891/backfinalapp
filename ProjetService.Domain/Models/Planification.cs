using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetService.Domain.Models
{


    public enum EtatListe
    {
        Todo = 0,
        EnCours = 1,
        Test = 2,
        Termine = 3
    }
    public class Planification
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [Column("heure_debut")]
        public TimeSpan HeureDebut { get; set; }

        [Required]
        [Column("heure_fin")]
        public TimeSpan HeureFin { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [Required]
        [Column("tache_id")]
        public int TacheId { get; set; }

        [Required]
        [Column("projet_id")]
        public int ProjetId { get; set; }

        [Required]
        [Column("liste_id")]
        public EtatListe ListeId { get; set; } = EtatListe.Todo;  // Valeur par défaut : Todo
        [Required]
        [Column("user_id")]
        public int UserId { get; set; }

        [ForeignKey("TacheId")]
        public virtual Tache Tache { get; set; }

        [ForeignKey("ProjetId")]
        public virtual Projet Projet { get; set; }
    }
}

