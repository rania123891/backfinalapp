using System.Linq;
using Microsoft.EntityFrameworkCore;
using UserService.Data.Context;
using UserService.Domain.Interfaces;
using UserService.Domain.Models;

namespace UserService.Data.Repositories
{
    public class UtilisateurRepository : IUtilisateurRepository
    {
        private readonly UserDbContext _context;

        public UtilisateurRepository(UserDbContext context)
        {
            _context = context;
        }

        // ✅ Méthodes manquantes ajoutées :
        public Utilisateur GetById(int id) => _context.Utilisateurs.Find(id);

        public Utilisateur GetByEmail(string email) =>
            _context.Utilisateurs.FirstOrDefault(u => u.Email == email);
        public IEnumerable<Utilisateur> GetAll() =>
    _context.Utilisateurs.ToList();


        public void Add(Utilisateur utilisateur)
        {
            _context.Utilisateurs.Add(utilisateur);
            _context.SaveChanges();
        }

        public void Update(Utilisateur utilisateur)
        {
            _context.Utilisateurs.Update(utilisateur);
            _context.SaveChanges();
        }

        public void Delete(Utilisateur utilisateur)
        {
            _context.Utilisateurs.Remove(utilisateur);
            _context.SaveChanges();
        }
        public async Task<IEnumerable<Utilisateur>> GetAllAsync()
        {
            return await _context.Utilisateurs
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<Utilisateur> GetByIdAsync(int id)
        {
            return await _context.Utilisateurs.FindAsync(id);
        }

        public async Task UpdateAsync(Utilisateur utilisateur)
        {
            _context.Utilisateurs.Update(utilisateur);
            await _context.SaveChangesAsync();
        }
    }
}
