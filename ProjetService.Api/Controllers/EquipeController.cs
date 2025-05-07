using Microsoft.AspNetCore.Mvc;
using ProjetService.Domain.Models;
using ProjetService.Domain.Interfaces;
using MediatR;
using ProjetService.Domain.DTO;

namespace ProjetService.Api.Controllers
{
    [Route("api/equipes")]
    [ApiController]
    public class EquipeController : GenericController<Equipe>
    {
        private readonly IGenericRepository<Equipe> _equipeRepository;
        private readonly IGenericRepository<MembreEquipe> _membreEquipeRepository;

        public EquipeController(
            IMediator mediator,
            IGenericRepository<Equipe> equipeRepository,
            IGenericRepository<MembreEquipe> membreEquipeRepository)
            : base(mediator, equipeRepository)
        {
            _equipeRepository = equipeRepository;
            _membreEquipeRepository = membreEquipeRepository;
        }

        [HttpPost("{equipeId}/membres")]
        public async Task<IActionResult> AjouterMembres(int equipeId, [FromBody] AjouterMembreDto[] dto)
        {
            if (dto == null || dto.Length == 0)
                return BadRequest("Aucun membre à ajouter.");

            foreach (var membre in dto)
            {
                // Ici, ajoute chaque membre à l'équipe
                // Par exemple :
                var membreEquipe = new MembreEquipe
                {
                    EquipeId = equipeId,
                    UtilisateurId = membre.UtilisateurId,
                    Role = membre.Role,
                    DateAjout = DateTime.UtcNow
                };
                await _membreEquipeRepository.AddAsync(membreEquipe);
            }

            return Ok();
        }

        // DELETE: api/equipes/{equipeId}/membres/{membreEquipeId}
        [HttpDelete("{equipeId}/membres/{membreEquipeId}")]
        public async Task<IActionResult> SupprimerMembre(int equipeId, int membreEquipeId)
        {
            var membre = await _membreEquipeRepository.GetByIdAsync(membreEquipeId);
            if (membre == null || membre.EquipeId != equipeId)
                return NotFound("Membre non trouvé dans cette équipe.");

            await _membreEquipeRepository.DeleteByIdAsync(membreEquipeId);
            return Ok();
        }
    }


}