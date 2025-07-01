using ProjetService.Domain.Models;

namespace ProjetService.Domain.Dto
{
    public class CreatePlanificationDto
    {
        public DateTime Date { get; set; }
        public string HeureDebut { get; set; }
        public string HeureFin { get; set; }
        public string Description { get; set; }
        public int TacheId { get; set; }
        public int ProjetId { get; set; }
        public EtatListe ListeId { get; set; } = EtatListe.Todo;
        public int UserId { get; set; }
    }

    public class UpdatePlanificationDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string HeureDebut { get; set; }
        public string HeureFin { get; set; }
        public string Description { get; set; }
        public int TacheId { get; set; }
        public int ProjetId { get; set; }
        public EtatListe ListeId { get; set; }
        public int UserId { get; set; }
    }

    public class UpdatePlanificationStatusDto
    {
        public EtatListe ListeId { get; set; }
    }
}