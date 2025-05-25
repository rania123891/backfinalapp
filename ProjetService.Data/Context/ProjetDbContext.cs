using Microsoft.EntityFrameworkCore;
using ProjetService.Domain.Models;

namespace ProjetService.Data.Context
{
    public class ProjetDbContext : DbContext
    {
        public DbSet<Tache> Taches { get; set; }
        public DbSet<Equipe> Equipes { get; set; }
        public DbSet<Projet> Projets { get; set; }
        public DbSet<MembreEquipe> MembresEquipe { get; set; }
        public DbSet<Commentaire> Commentaires { get; set; }
        public DbSet<Planification> Planifications { get; set; }

        public ProjetDbContext(DbContextOptions<ProjetDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ✅ Configuration Planification
            modelBuilder.Entity<Planification>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Date)
                    .IsRequired();

                entity.Property(e => e.HeureDebut)
                    .IsRequired()
                    .HasColumnName("heure_debut");

                entity.Property(e => e.HeureFin)
                    .IsRequired()
                    .HasColumnName("heure_fin");

                entity.Property(e => e.Description)
                    .HasMaxLength(1000);

                entity.Property(e => e.TacheId)
                    .IsRequired()
                    .HasColumnName("tache_id");

                entity.Property(e => e.ProjetId)
                    .IsRequired()
                    .HasColumnName("projet_id");

                entity.Property(e => e.ListeId)
                    .IsRequired()
                    .HasColumnName("liste_id")
                    .HasDefaultValue(EtatListe.Todo);

                // Relations Planification
                entity.HasOne(d => d.Tache)
                    .WithMany(t => t.Planifications)
                    .HasForeignKey(d => d.TacheId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Projet)
                    .WithMany(p => p.Planifications)
                    .HasForeignKey(d => d.ProjetId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Index pour améliorer les performances
                entity.HasIndex(e => e.Date)
                    .HasDatabaseName("IX_Planifications_Date");

                entity.HasIndex(e => new { e.Date, e.HeureDebut })
                    .HasDatabaseName("IX_Planifications_Date_HeureDebut");
            });

            // ✅ Configuration Projet
            modelBuilder.Entity<Projet>(entity =>
            {
                entity.Property(p => p.CreateurId).IsRequired();

                // Relation Projet → Equipes
                entity.HasMany(p => p.Equipes)
                    .WithOne(e => e.Projet)
                    .HasForeignKey(e => e.ProjetId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ✅ Configuration Tache
            modelBuilder.Entity<Tache>(entity =>
            {
                // Relation Tache → Commentaires
                entity.HasMany(t => t.Commentaires)
                    .WithOne(c => c.Tache)
                    .HasForeignKey(c => c.TacheId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ✅ Configuration MembreEquipe
            modelBuilder.Entity<MembreEquipe>(entity =>
            {
                entity.Property(me => me.UtilisateurId).IsRequired();

                // Relation Equipe → MembresEquipe
                entity.HasOne(me => me.Equipe)
                    .WithMany(e => e.MembresEquipe)
                    .HasForeignKey(me => me.EquipeId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ✅ Configuration Commentaire
            modelBuilder.Entity<Commentaire>(entity =>
            {
                entity.Property(c => c.UtilisateurId).IsRequired();
            });
        }
    }
}