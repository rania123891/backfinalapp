using Microsoft.AspNetCore.Mvc;
using ProjetService.Domain.Models;
using ProjetService.Domain.Interfaces;
using MediatR;
using ProjetService.Domain.DTO;
using Microsoft.EntityFrameworkCore;
using ProjetService.Domain.Models.ProjetService.Domain.Models;

namespace ProjetService.Api.Controllers
{
    [Route("api/equipes")]
    [ApiController]
    public class EquipeController : GenericController<Equipe>
    {
        private readonly IGenericRepository<Equipe> _equipeRepository;
        private readonly IGenericRepository<MembreEquipe> _membreEquipeRepository;
        private readonly IGenericRepository<ProjetEquipe> _projetEquipeRepository;


        public EquipeController(
        IMediator mediator,
        IGenericRepository<Equipe> equipeRepository,
        IGenericRepository<MembreEquipe> membreEquipeRepository,
        IGenericRepository<ProjetEquipe> projetEquipeRepository)
        : base(mediator, equipeRepository)
        {
            _equipeRepository = equipeRepository;
            _membreEquipeRepository = membreEquipeRepository;
            _projetEquipeRepository = projetEquipeRepository;
        }

        // GET: api/equipes/membres
        [HttpGet("membres")]
        public async Task<ActionResult<IEnumerable<MembreEquipe>>> GetAllMembres()
        {
            var membres = await _membreEquipeRepository.GetAllAsync();
            return Ok(membres);
        }

        // GET: api/equipes/{equipeId}/membres
        [HttpGet("{equipeId}/membres")]
        public async Task<ActionResult<IEnumerable<MembreEquipe>>> GetMembresByEquipe(int equipeId)
        {
            var membres = await _membreEquipeRepository.GetAllAsync();
            var membresDeLEquipe = membres.Where(m => m.EquipeId == equipeId).ToList();
            return Ok(membresDeLEquipe);
        }

        [HttpPost("{equipeId}/membres")]
        public async Task<IActionResult> AjouterMembres(int equipeId, [FromBody] AjouterMembreDto[] dto)
        {
            if (dto == null || dto.Length == 0)
                return BadRequest("Aucun membre à ajouter.");

            foreach (var membre in dto)
            {
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

        [HttpDelete("{equipeId}/membres/{membreEquipeId}")]
        public async Task<IActionResult> SupprimerMembre(int equipeId, int membreEquipeId)
        {
            var membre = await _membreEquipeRepository.GetByIdAsync(membreEquipeId);
            if (membre == null || membre.EquipeId != equipeId)
                return NotFound("Membre non trouvé dans cette équipe.");

            await _membreEquipeRepository.DeleteByIdAsync(membreEquipeId);
            return Ok();
        }
        [HttpPost("{equipeId}/affecter-projet")]
        public async Task<IActionResult> AffecterProjet(int equipeId, [FromBody] int projetId)
        {
            // Vérifie que cette liaison n’existe pas déjà
            var existingRelations = await _projetEquipeRepository.GetAllAsync();
            bool existe = existingRelations.Any(pe => pe.ProjetId == projetId && pe.EquipeId == equipeId);

            if (existe)
                return BadRequest("Cette équipe est déjà affectée à ce projet.");

            // Crée la liaison
            var projetEquipe = new ProjetEquipe
            {
                ProjetId = projetId,
                EquipeId = equipeId
            };

            await _projetEquipeRepository.AddAsync(projetEquipe);

            return Ok("Equipe affectée au projet.");
        }
        [HttpGet("{equipeId}/projets")]
        public async Task<IActionResult> GetProjetsDeLEquipe(int equipeId)
        {
            var relations = await _projetEquipeRepository.GetAllAsync();
            var projetsIds = relations
                .Where(pe => pe.EquipeId == equipeId)
                .Select(pe => pe.ProjetId)
                .ToList();

            return Ok(projetsIds); // ou retourne directement les projets si tu charges les entités
        }


    }
}