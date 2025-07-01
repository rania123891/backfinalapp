using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjetService.Domain.Interfaces;
using ProjetService.Domain.Models;

namespace ProjetService.Domain.Interface
{
    public interface IPlanificationRepository : IGenericRepository<Planification>
    {
        Task<IEnumerable<Planification>> GetByDateAsync(DateTime date);
        Task<Planification> GetByIdWithIncludesAsync(int id);
        Task<IEnumerable<Planification>> GetAllWithIncludesAsync();
        Task<Planification> UpdateStatusAsync(int id, EtatListe nouveauStatut);
        Task<Planification> CreateWithIncludesAsync(Planification planification);
        Task<Planification> UpdateWithIncludesAsync(Planification planification);
        Task<IEnumerable<Planification>> GetByUserIdAsync(int userId);
        Task<IEnumerable<Planification>> GetByUserIdAndDateAsync(int userId, DateTime date);
        Task<IEnumerable<Planification>> GetAllByUserIdWithIncludesAsync(int userId);
       
    }
}
