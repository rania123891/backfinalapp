using UserService.Domain.Models;

namespace UserService.Domain.Interfaces
{
    public interface IUtilisateurRepository
    {
        Utilisateur GetById(int id);
        Utilisateur GetByEmail(string email);
        IEnumerable<Utilisateur> GetAll();
        void Add(Utilisateur utilisateur);
        void Update(Utilisateur utilisateur);
        void Delete(Utilisateur utilisateur);
        Task<IEnumerable<Utilisateur>> GetAllAsync();

    }
}
