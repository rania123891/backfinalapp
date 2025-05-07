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
        public DbSet<Tableau> Tableaux { get; set; }
        public DbSet<Commentaire> Commentaires { get; set; }
        public DbSet<Liste> Listes { get; set; }

        public ProjetDbContext(DbContextOptions<ProjetDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ✅ Relation Projet → Taches
            modelBuilder.Entity<Projet>()
                .HasMany(p => p.Taches)
                .WithOne(t => t.Projet)
                .HasForeignKey(t => t.ProjetId)
                .OnDelete(DeleteBehavior.Restrict);

            // ✅ Relation Projet → Equipes
            modelBuilder.Entity<Projet>()
                .HasMany(p => p.Equipes)
                .WithOne(e => e.Projet)
                .HasForeignKey(e => e.ProjetId)
                .OnDelete(DeleteBehavior.Cascade);

            // ✅ Relation Projet → Tableaux
            modelBuilder.Entity<Projet>()
                .HasMany(p => p.Tableaux)
                .WithOne(t => t.Projet)
                .HasForeignKey(t => t.ProjetId)
                .OnDelete(DeleteBehavior.Cascade);

            // ✅ Relation Tableau → Listes
            modelBuilder.Entity<Tableau>()
                .HasMany(t => t.Listes)
                .WithOne(l => l.Tableau)
                .HasForeignKey(l => l.TableauId)
                .OnDelete(DeleteBehavior.Cascade);

            // ✅ Relation Liste → Taches
            modelBuilder.Entity<Liste>()
                .HasMany(l => l.Taches)
                .WithOne(t => t.Liste)
                .HasForeignKey(t => t.ListeId)
                .OnDelete(DeleteBehavior.Cascade);

            // ✅ Relation Tache → Commentaires
            modelBuilder.Entity<Tache>()
                .HasMany(t => t.Commentaires)
                .WithOne(c => c.Tache)
                .HasForeignKey(c => c.TacheId)
                .OnDelete(DeleteBehavior.Restrict);

            // ✅ Relation Equipe → MembresEquipe
            modelBuilder.Entity<MembreEquipe>()
                .HasOne(me => me.Equipe)
                .WithMany(e => e.MembresEquipe)
                .HasForeignKey(me => me.EquipeId)
                .OnDelete(DeleteBehavior.Cascade);

            // ✅ Stockage des IDs utilisateurs (UserService)
            modelBuilder.Entity<Projet>().Property(p => p.CreateurId).IsRequired();
            modelBuilder.Entity<Tache>().Property(t => t.AssigneId).IsRequired();
            modelBuilder.Entity<Commentaire>().Property(c => c.UtilisateurId).IsRequired();
            modelBuilder.Entity<MembreEquipe>().Property(me => me.UtilisateurId).IsRequired();
        }
    }
}