using Microsoft.EntityFrameworkCore;
using MessageService.Domain.Models;

namespace MessageService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Message> Messages { get; set; }
        public DbSet<MessageAttachment> MessageAttachments { get; set; }
        public DbSet<FileUpload> FileUploads { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuration de l'entité Message
            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasKey(m => m.Id);
                entity.Property(m => m.Contenu).IsRequired();
                entity.Property(m => m.ExpediteurId).IsRequired();
                entity.Property(m => m.DestinataireId).IsRequired();
                entity.Property(m => m.EnvoyeLe).IsRequired();
                entity.Property(m => m.Lu).IsRequired();
            });

            // Configuration de l'entité MessageAttachment
            modelBuilder.Entity<MessageAttachment>(entity =>
            {
                entity.HasKey(ma => ma.Id);
                entity.Property(ma => ma.FileName).IsRequired().HasMaxLength(255);
                entity.Property(ma => ma.OriginalFileName).IsRequired().HasMaxLength(255);
                entity.Property(ma => ma.FilePath).IsRequired().HasMaxLength(500);
                entity.Property(ma => ma.FileType).IsRequired().HasMaxLength(50);
                entity.Property(ma => ma.MimeType).IsRequired().HasMaxLength(100);
                entity.Property(ma => ma.UploadedBy).IsRequired().HasMaxLength(50);

                // Configuration explicite de la relation ONE-TO-MANY
                entity.HasOne(ma => ma.Message)
                      .WithMany(m => m.Attachments)
                      .HasForeignKey(ma => ma.MessageId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuration de l'entité FileUpload
            modelBuilder.Entity<FileUpload>(entity =>
            {
                entity.HasKey(f => f.Id);
                entity.Property(f => f.Id).IsRequired().HasMaxLength(255);
                entity.Property(f => f.OriginalName).IsRequired().HasMaxLength(255);
                entity.Property(f => f.StoredName).IsRequired().HasMaxLength(255);
                entity.Property(f => f.FilePath).IsRequired().HasMaxLength(500);
                entity.Property(f => f.MimeType).IsRequired().HasMaxLength(100);
                entity.Property(f => f.UploadedBy).IsRequired().HasMaxLength(50);
                entity.Property(f => f.Size).IsRequired();
                entity.Property(f => f.UploadDate).IsRequired();
            });
        }
    }
}