// ProjetService.Domain/Models/EquipeUtilisateur.cs
using ProjetService.Domain.Models;

public class EquipeUtilisateur
{
    public int EquipeId { get; set; }
    public int UtilisateurId { get; set; }
    public string Role { get; set; }
    public DateTime DateAjout { get; set; } = DateTime.Now;

    public virtual Equipe Equipe { get; set; }
}