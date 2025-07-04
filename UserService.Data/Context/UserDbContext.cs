﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserService.Domain.Models;

namespace UserService.Data.Context
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }

        public DbSet<Utilisateur> Utilisateurs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Utilisateur>(entity =>
            {
                // Configuration de la table
                entity.ToTable("Utilisateurs");

                // Clé primaire
                entity.HasKey(e => e.Id);

                // Configuration des propriétés
                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(255);

                // Configuration de la nouvelle propriété ProfilePhotoUrl
                entity.Property(e => e.ProfilePhotoUrl)
                    .HasMaxLength(500)  // URL peut être assez longue
                    .IsRequired(false); // La photo de profil est optionnelle

                // Ajoute un index sur l'email pour des recherches plus rapides
                entity.HasIndex(e => e.Email)
                    .IsUnique();
            });
        }
    }
}