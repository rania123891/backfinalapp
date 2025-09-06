using Microsoft.EntityFrameworkCore;
using ProjetService.Data.Context;
using ProjetService.Domain.DTO;
using ProjetService.Domain.Interface;
using ProjetService.Domain.Models;

namespace ProjetService.Data.Repositories
{
    public class SaisieTempsRepository : ISaisieTempsRepository
    {
        private readonly ProjetDbContext _context;

        public SaisieTempsRepository(ProjetDbContext context)
        {
            _context = context;
        }

        public async Task<SaisieTemps> GetByIdAsync(int id)
        {
            return await _context.SaisiesTemps.FindAsync(id);
        }

        public async Task<List<SaisieTemps>> GetByUtilisateurIdAsync(int utilisateurId, DateTime? dateDebut = null, DateTime? dateFin = null)
        {
            var query = _context.SaisiesTemps
                .Where(s => s.UtilisateurId == utilisateurId);

            if (dateDebut.HasValue)
                query = query.Where(s => s.DateTravail >= dateDebut.Value);

            if (dateFin.HasValue)
                query = query.Where(s => s.DateTravail <= dateFin.Value);

            return await query
                .OrderByDescending(s => s.DateTravail)
                .ToListAsync();
        }

        public async Task<List<SaisieTemps>> GetByProjetIdAsync(int projetId, DateTime? dateDebut = null, DateTime? dateFin = null)
        {
            var query = _context.SaisiesTemps
                .Where(s => s.ProjetId == projetId);

            if (dateDebut.HasValue)
                query = query.Where(s => s.DateTravail >= dateDebut.Value);

            if (dateFin.HasValue)
                query = query.Where(s => s.DateTravail <= dateFin.Value);

            return await query
                .OrderByDescending(s => s.DateTravail)
                .ToListAsync();
        }

        public async Task<List<SaisieTemps>> GetByTacheIdAsync(int tacheId, DateTime? dateDebut = null, DateTime? dateFin = null)
        {
            var query = _context.SaisiesTemps
                .Where(s => s.TacheId == tacheId);

            if (dateDebut.HasValue)
                query = query.Where(s => s.DateTravail >= dateDebut.Value);

            if (dateFin.HasValue)
                query = query.Where(s => s.DateTravail <= dateFin.Value);

            return await query
                .OrderByDescending(s => s.DateTravail)
                .ToListAsync();
        }

        public async Task<int> CreateAsync(SaisieTemps saisieTemps)
        {
            _context.SaisiesTemps.Add(saisieTemps);
            await _context.SaveChangesAsync();
            return saisieTemps.Id;
        }

        public async Task<bool> UpdateAsync(SaisieTemps saisieTemps)
        {
            _context.SaisiesTemps.Update(saisieTemps);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var saisieTemps = await _context.SaisiesTemps.FindAsync(id);
            if (saisieTemps == null)
                return false;

            _context.SaisiesTemps.Remove(saisieTemps);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.SaisiesTemps.AnyAsync(s => s.Id == id);
        }

        public async Task<decimal> GetTotalHeuresParProjetAsync(int projetId, DateTime? dateDebut = null, DateTime? dateFin = null)
        {
            var query = _context.SaisiesTemps
                .Where(s => s.ProjetId == projetId);

            if (dateDebut.HasValue)
                query = query.Where(s => s.DateTravail >= dateDebut.Value);

            if (dateFin.HasValue)
                query = query.Where(s => s.DateTravail <= dateFin.Value);

            return await query.SumAsync(s => s.DureeHeures);
        }

        // ✅ CORRIGÉ : Récupérer les heures par utilisateur (sans jointure Utilisateurs car microservice)
        public async Task<Dictionary<string, decimal>> GetHeuresParUtilisateurAsync(int projetId, DateTime? dateDebut = null, DateTime? dateFin = null)
        {
            var query = _context.SaisiesTemps
                .Where(s => s.ProjetId == projetId);

            if (dateDebut.HasValue)
                query = query.Where(s => s.DateTravail >= dateDebut.Value);

            if (dateFin.HasValue)
                query = query.Where(s => s.DateTravail <= dateFin.Value);

            // Grouper par UtilisateurId - les noms seront enrichis côté frontend
            var result = await query
                .GroupBy(s => s.UtilisateurId)
                .Select(g => new { UtilisateurId = g.Key, TotalHeures = g.Sum(x => x.DureeHeures) })
                .ToDictionaryAsync(x => $"Utilisateur_{x.UtilisateurId}", x => x.TotalHeures);

            return result;
        }

        // ✅ CORRIGÉ : Utiliser Titre au lieu de Nom pour les tâches
        public async Task<Dictionary<string, decimal>> GetHeuresParTacheAsync(int projetId, DateTime? dateDebut = null, DateTime? dateFin = null)
        {
            var query = _context.SaisiesTemps
                .Where(s => s.ProjetId == projetId && s.TacheId.HasValue);

            if (dateDebut.HasValue)
                query = query.Where(s => s.DateTravail >= dateDebut.Value);

            if (dateFin.HasValue)
                query = query.Where(s => s.DateTravail <= dateFin.Value);

            // ✅ UTILISER Titre au lieu de Nom
            var result = await query
                .Join(_context.Taches,
                      s => s.TacheId,
                      t => t.Id,
                      (s, t) => new { s.DureeHeures, NomTache = t.Titre })  // ✅ Titre au lieu de Nom
                .GroupBy(x => x.NomTache)
                .Select(g => new { NomTache = g.Key, TotalHeures = g.Sum(x => x.DureeHeures) })
                .ToDictionaryAsync(x => x.NomTache, x => x.TotalHeures);

            return result;
        }

        // ✅ CORRIGÉ : Récupérer les saisies avec les noms pour les rapports (sans Utilisateurs)
        public async Task<List<SaisieTempsDto>> GetSaisiesAvecNomsAsync(int projetId, DateTime? dateDebut = null, DateTime? dateFin = null)
        {
            var query = _context.SaisiesTemps
                .Where(s => s.ProjetId == projetId);

            if (dateDebut.HasValue)
                query = query.Where(s => s.DateTravail >= dateDebut.Value);

            if (dateFin.HasValue)
                query = query.Where(s => s.DateTravail <= dateFin.Value);

            var result = await query
                .Join(_context.Projets, s => s.ProjetId, p => p.Id, (s, p) => new { s, NomProjet = p.Nom })
                .GroupJoin(_context.Taches, sp => sp.s.TacheId, t => t.Id, (sp, taches) => new { sp.s, sp.NomProjet, NomTache = taches.FirstOrDefault().Titre })  // ✅ Titre au lieu de Nom
                .Select(x => new SaisieTempsDto
                {
                    Id = x.s.Id,
                    UtilisateurId = x.s.UtilisateurId,
                    ProjetId = x.s.ProjetId,
                    TacheId = x.s.TacheId,
                    DateTravail = x.s.DateTravail,
                    HeureDebut = x.s.HeureDebut,
                    HeureFin = x.s.HeureFin,
                    DureeHeures = x.s.DureeHeures,
                    Description = x.s.Description,
                    CreatedAt = x.s.CreatedAt,
                    NomProjet = x.NomProjet,
                    NomUtilisateur = $"Utilisateur_{x.s.UtilisateurId}",  // ✅ Pas de jointure Utilisateurs
                    NomTache = x.NomTache ?? "Aucune tâche"
                })
                .OrderByDescending(s => s.DateTravail)
                .ToListAsync();

            return result;
        }

        // ✅ CORRIGÉ : Récupérer les données de planification avec noms (sans Utilisateurs)
        public async Task<List<PlanificationDto>> GetPlanificationsAvecNomsAsync(int projetId, DateTime? dateDebut = null, DateTime? dateFin = null)
        {
            var query = _context.Planifications
                .Where(p => p.ProjetId == projetId);

            if (dateDebut.HasValue)
                query = query.Where(p => p.Date >= dateDebut.Value);

            if (dateFin.HasValue)
                query = query.Where(p => p.Date <= dateFin.Value);

            var result = await query
                .Join(_context.Projets, p => p.ProjetId, proj => proj.Id, (p, proj) => new { p, NomProjet = proj.Nom })
                .Join(_context.Taches, pp => pp.p.TacheId, t => t.Id, (pp, t) => new { pp.p, pp.NomProjet, NomTache = t.Titre })  // ✅ Titre au lieu de Nom
                .Select(x => new PlanificationDto
                {
                    Id = x.p.Id,
                    Date = x.p.Date,
                    HeureDebut = x.p.HeureDebut.ToString(@"hh\:mm"),
                    HeureFin = x.p.HeureFin.ToString(@"hh\:mm"),
                    DureeHeures = (decimal)(x.p.HeureFin - x.p.HeureDebut).TotalHours,
                    Description = x.p.Description,
                    TacheId = x.p.TacheId,
                    ProjetId = x.p.ProjetId,
                    UserId = x.p.UserId,
                    NomProjet = x.NomProjet,
                    NomTache = x.NomTache,
                    NomUtilisateur = $"Utilisateur_{x.p.UserId}"  // ✅ Pas de jointure Utilisateurs
                })
                .OrderBy(p => p.Date)
                .ToListAsync();

            return result;
        }
    }
}

