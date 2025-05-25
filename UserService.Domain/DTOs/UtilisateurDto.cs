namespace UserService.Domain.DTOs
{
    public class UtilisateurDto
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Nom { get; set; }    // Ajouté
        public string Prenom { get; set; }  // Ajouté
        public string Role { get; set; }
        public string? ProfilePhotoUrl { get; set; }
    }
}