using Microsoft.EntityFrameworkCore;
using ProjetService.Data.Context;
using ProjetService.Domain.Interface;
using ProjetService.Domain.Interfaces;
using ProjetService.Domain.Models;

namespace ProjetService.Data.Repositories
{
    public class PlanificationRepository : GenericRepository<Planification>, IPlanificationRepository
    {
        private readonly ProjetDbContext _context;

        public PlanificationRepository(ProjetDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Planification>> GetByDateAsync(DateTime date)
        {
            return await _context.Planifications
                .Include(p => p.Tache)
                .Include(p => p.Projet)
                .Where(p => p.Date.Date == date.Date)
                .OrderBy(p => p.HeureDebut)
                .ToListAsync();
        }

        public async Task<Planification> GetByIdWithIncludesAsync(int id)
        {
            return await _context.Planifications
                .Include(p => p.Tache)
                .Include(p => p.Projet)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Planification>> GetAllWithIncludesAsync()
        {
            return await _context.Planifications
                .Include(p => p.Tache)
                .Include(p => p.Projet)
                .OrderBy(p => p.Date)
                .ThenBy(p => p.HeureDebut)
                .ToListAsync();
        }

        public async Task<Planification> UpdateStatusAsync(int id, EtatListe nouveauStatut)
        {
            var planification = await GetByIdWithIncludesAsync(id);
            if (planification == null)
                return null;

            planification.ListeId = nouveauStatut;
            await _context.SaveChangesAsync();

            return planification;
        }

        public async Task<Planification> CreateWithIncludesAsync(Planification planification)
        {
            await AddAsync(planification);
            return await GetByIdWithIncludesAsync(planification.Id);
        }

        public async Task<Planification> UpdateWithIncludesAsync(Planification planification)
        {
            await UpdateAsync(planification);
            return await GetByIdWithIncludesAsync(planification.Id);
        }
        public async Task<IEnumerable<Planification>> GetByUserIdAsync(int userId)
        {
            return await _context.Planifications
                .Include(p => p.Tache)
                .Include(p => p.Projet)
                .Where(p => p.UserId == userId)
                .OrderBy(p => p.Date)
                .ThenBy(p => p.HeureDebut)
                .ToListAsync();
        }

        public async Task<IEnumerable<Planification>> GetByUserIdAndDateAsync(int userId, DateTime date)
        {
            return await _context.Planifications
                .Include(p => p.Tache)
                .Include(p => p.Projet)
                .Where(p => p.UserId == userId && p.Date.Date == date.Date)
                .OrderBy(p => p.HeureDebut)
                .ToListAsync();
        }

        public async Task<IEnumerable<Planification>> GetAllByUserIdWithIncludesAsync(int userId)
        {
            return await _context.Planifications
                .Include(p => p.Tache)
                .Include(p => p.Projet)
                .Where(p => p.UserId == userId)
                .OrderBy(p => p.Date)
                .ThenBy(p => p.HeureDebut)
                .ToListAsync();
        }
      
    }
}
