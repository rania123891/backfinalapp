namespace UserService.Domain.Models
{
    public enum RoleUtilisateur
    {
        Admin,
        User
    }

    public class Utilisateur
    {
        public int Id { get; set; }
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }

        // ✅ Correction ici : Utilisation correcte de RoleUtilisateur
        public RoleUtilisateur Role { get; set; } = RoleUtilisateur.User;

        public DateTime DateCreation { get; set; } = DateTime.UtcNow;
        public string? ProfilePhotoUrl { get; set; }  // Ajout de la nouvelle propriété
    }
}
