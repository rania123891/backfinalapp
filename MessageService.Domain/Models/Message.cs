using System.ComponentModel.DataAnnotations;

namespace MessageService.Domain.Models
{
    public class Message
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Contenu { get; set; } = string.Empty;

        [Required]
        public string ExpediteurId { get; set; } = string.Empty;

        [Required]
        public string DestinataireId { get; set; } = string.Empty;

        public DateTime EnvoyeLe { get; set; }

        public bool Lu { get; set; }

        // Navigation property pour les attachements
        public virtual ICollection<MessageAttachment>? Attachments { get; set; }
    }
}