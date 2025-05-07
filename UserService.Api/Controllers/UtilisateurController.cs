// UserService.Api/Controllers/UtilisateurController.cs
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Domain.Commands;
using UserService.Domain.DTOs; // Créez ce dossier pour vos DTOs si nécessaire
using UserService.Domain.Models;
using UserService.Domain.Queries;
using UserService.Domain.Interfaces;

namespace UserService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UtilisateurController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IUtilisateurRepository _repository;
        private readonly IAuthService _authService;

        public UtilisateurController(IMediator mediator, IUtilisateurRepository repository, IAuthService authService)
        {
            _mediator = mediator;
            _repository = repository;
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterDto registerDto)
        {
            var command = new CreateUtilisateurCommand(
                registerDto.Email,
                registerDto.Password,
                registerDto.Role,
                registerDto.Nom,
                registerDto.Prenom
            );

            try
            {
                var utilisateur = await _mediator.Send(command);
                return Ok("Inscription réussie.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        // ... existing code ...

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto loginDto)
        {
            var utilisateur = _repository.GetByEmail(loginDto.Email);
            if (utilisateur == null || !_authService.VerifyPassword(loginDto.Password, utilisateur.PasswordHash))
                return Unauthorized("Email ou mot de passe incorrect.");

            var token = _authService.GenerateJwtToken(utilisateur);

            return Ok(new
            {
                token = token,
                id = utilisateur.Id,
                nom = utilisateur.Nom,
                prenom = utilisateur.Prenom,
                email = utilisateur.Email
            });
        }

        // ... existing code ...

        // Exemple d'endpoint protégé (admin uniquement)
        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("{id}")]
        public IActionResult DeleteUtilisateur(int id)
        {
            var utilisateur = _repository.GetById(id);
            if (utilisateur == null)
                return NotFound("Utilisateur non trouvé.");

            _repository.Delete(utilisateur);
            return Ok("Utilisateur supprimé.");
        }

        // Endpoint de récupération d'un utilisateur via CQRS (accessible aux utilisateurs authentifiés)
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUtilisateur(int id)
        {
            var query = new GetUtilisateurQuery(id);
            var utilisateur = await _mediator.Send(query);
            if (utilisateur == null)
                return NotFound();
            return Ok(utilisateur);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpGet]
        public async Task<IActionResult> GetAllUtilisateurs()
        {
            var query = new GetAllUtilisateursQuery();
            var utilisateurs = await _mediator.Send(query);

            var dtos = utilisateurs.Select(u => new UtilisateurDto
            {
                Id = u.Id,
                Email = u.Email,
                Role = u.Role.ToString()
            });

            return Ok(dtos);
        }
    }
}
