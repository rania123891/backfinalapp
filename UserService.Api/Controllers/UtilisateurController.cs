using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserService.Domain.Commands;
using UserService.Domain.DTOs;
using UserService.Domain.Queries;
using UserService.Domain.Interfaces;
using UserService.Data.Context;
using System.Security.Claims;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Linq;
using UserService.Domain.Models;

namespace UserService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UtilisateurController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IUtilisateurRepository _repository;
        private readonly IAuthService _authService;
        private readonly UserDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<UtilisateurController> _logger;

        public UtilisateurController(
            IMediator mediator,
            IUtilisateurRepository repository,
            IAuthService authService,
            UserDbContext context,
            IWebHostEnvironment environment,
            ILogger<UtilisateurController> logger)
        {
            _mediator = mediator;
            _repository = repository;
            _authService = authService;
            _context = context;
            _environment = environment;
            _logger = logger;
        }

        private bool IsCurrentUserAdmin()
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var isAdmin = userRole?.ToLower() == "admin" || userRole?.ToLower() == "administrateur";
            _logger.LogInformation($"👑 Vérification admin - Role: {userRole}, Est admin: {isAdmin}");
            return isAdmin;
        }
        [HttpPost("register")]
public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
{
    _logger.LogInformation($"📝 Tentative d'inscription pour: {registerDto.Email}");

    try
    {
        // 1. Création de l'utilisateur
        var command = new CreateUtilisateurCommand(
            registerDto.Email,
            registerDto.Password,
            registerDto.Role,
            registerDto.Nom,
            registerDto.Prenom
        );

        var utilisateur = await _mediator.Send(command);

        // Inscription simple sans vérification d'email

        _logger.LogInformation($"✅ Inscription réussie pour: {registerDto.Email}");

        return Ok(new
        {
            message = "Inscription réussie ! Vous pouvez maintenant vous connecter.",
            success = true
        });
    }
    catch (Exception ex)
    {
        _logger.LogError($"❌ Échec de l'inscription pour {registerDto.Email}: {ex.Message}");
        return BadRequest(ex.Message);
    }
}

        /*  [HttpPost("register")]
           public async Task<IActionResult> Register([FromForm] RegisterDto registerDto)
           {
               _logger.LogInformation($"📝 Tentative d'inscription pour: {registerDto.Email}");

               try
               {
                   var command = new CreateUtilisateurCommand(
                       registerDto.Email,
                       registerDto.Password,
                       registerDto.Role,
                       registerDto.Nom,
                       registerDto.Prenom
                   );

                   var utilisateur = await _mediator.Send(command);
                   _logger.LogInformation($"✅ Inscription réussie pour: {registerDto.Email}");
                   return Ok("Inscription réussie.");
               }
               catch (Exception ex)
               {
                   _logger.LogError($"❌ Échec de l'inscription pour {registerDto.Email}: {ex.Message}");
                   return BadRequest(ex.Message);
               }
           }*/

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto loginDto)
        {
            _logger.LogInformation($"🔑 Tentative de connexion pour: {loginDto.Email}");

            var utilisateur = _repository.GetByEmail(loginDto.Email);
            if (utilisateur == null)
            {
                _logger.LogWarning($"❌ Utilisateur non trouvé: {loginDto.Email}");
                return Unauthorized("Email ou mot de passe incorrect.");
            }

            if (!_authService.VerifyPassword(loginDto.Password, utilisateur.PasswordHash))
            {
                _logger.LogWarning($"❌ Mot de passe incorrect pour: {loginDto.Email}");
                return Unauthorized("Email ou mot de passe incorrect.");
            }

            var token = _authService.GenerateJwtToken(utilisateur);
            _logger.LogInformation($"✅ Connexion réussie pour: {loginDto.Email}, Role: {utilisateur.Role}");

            return Ok(new
            {
                token = token,
                id = utilisateur.Id,
                nom = utilisateur.Nom,
                prenom = utilisateur.Prenom,
                email = utilisateur.Email,
                role = utilisateur.Role.ToString()
            });
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<UtilisateurDto>> GetUtilisateur(int id)
        {
            _logger.LogInformation($"🔍 Recherche utilisateur ID: {id}");

            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            _logger.LogInformation($"🔍 Claims utilisateur courant - NameIdentifier: {currentUserId}");

            if (!IsCurrentUserAdmin() && currentUserId != id.ToString())
            {
                _logger.LogWarning($"🚫 Accès non autorisé - User {currentUserId} tente d'accéder à {id}");
                return Forbid();
            }

            var utilisateur = await _context.Utilisateurs.FindAsync(id);
            if (utilisateur == null)
            {
                _logger.LogWarning($"❌ Utilisateur non trouvé: {id}");
                return NotFound();
            }

            _logger.LogInformation($"✅ Utilisateur trouvé: {id}");
            return Ok(new UtilisateurDto
            {
                Id = utilisateur.Id,
                Email = utilisateur.Email,
                Nom = utilisateur.Nom,
                Prenom = utilisateur.Prenom,
                Role = utilisateur.Role.ToString(),
                ProfilePhotoUrl = utilisateur.ProfilePhotoUrl
            });
        }

        [HttpGet("by-email/{email}")]
        [Authorize]
        public async Task<ActionResult<UtilisateurDto>> GetByEmail(string email)
        {
            _logger.LogInformation($"🔍 Recherche utilisateur par email: {email}");

            if (!IsCurrentUserAdmin())
            {
                var currentUserEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                _logger.LogInformation($"🔍 Claims email utilisateur courant: {currentUserEmail}");

                if (currentUserEmail != email)
                {
                    _logger.LogWarning($"🚫 Accès non autorisé à l'email: {email}");
                    return Forbid();
                }
            }

            var utilisateur = await _context.Utilisateurs
                .FirstOrDefaultAsync(u => u.Email == email);

            if (utilisateur == null)
            {
                _logger.LogWarning($"❌ Utilisateur non trouvé: {email}");
                return NotFound();
            }

            _logger.LogInformation($"✅ Utilisateur trouvé: {email}");
            return Ok(new UtilisateurDto
            {
                Id = utilisateur.Id,
                Email = utilisateur.Email,
                Nom = utilisateur.Nom,
                Prenom = utilisateur.Prenom,
                Role = utilisateur.Role.ToString()
            });
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpGet]
        public async Task<IActionResult> GetAllUtilisateurs()
        {
            _logger.LogInformation("📋 Récupération de tous les utilisateurs");

            try
            {
                var query = new GetAllUtilisateursQuery();
                var utilisateurs = await _mediator.Send(query);

                var dtos = utilisateurs.Select(u => new UtilisateurDto
                {
                    Id = u.Id,
                    Email = u.Email,
                    Nom = u.Nom,
                    Prenom = u.Prenom,
                    Role = u.Role.ToString()
                });

                _logger.LogInformation($"✅ {utilisateurs.Count()} utilisateurs récupérés");
                return Ok(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Erreur lors de la récupération des utilisateurs: {ex.Message}");
                return StatusCode(500, "Erreur lors de la récupération des utilisateurs");
            }
        }

        [HttpGet("for-selection")]
        [Authorize(Roles = "Admin,Administrateur")]
        public async Task<ActionResult> GetUsersForSelection()
        {
            _logger.LogInformation("📋 Récupération des utilisateurs pour sélection");

            // Debug des claims
            foreach (var claim in User.Claims)
            {
                _logger.LogInformation($"🔍 Claim: {claim.Type} = {claim.Value}");
            }

            try
            {
                var utilisateurs = await _context.Utilisateurs
                    .Select(u => new {
                        u.Id,
                        u.Email,
                        u.Nom,
                        u.Prenom,
                        FullName = $"{u.Prenom} {u.Nom}",
                        DisplayText = $"{u.Prenom} {u.Nom} ({u.Email})",
                        Role = u.Role.ToString()
                    })
                    .OrderBy(u => u.Nom)
                    .ThenBy(u => u.Prenom)
                    .ToListAsync();

                _logger.LogInformation($"✅ {utilisateurs.Count} utilisateurs récupérés pour sélection");
                return Ok(utilisateurs);
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Erreur lors de la récupération des utilisateurs: {ex.Message}");
                return StatusCode(500, "Erreur lors de la récupération des utilisateurs");
            }
        }

        [HttpGet("search")]
        [Authorize(Roles = "Admin,Administrateur")]
        public async Task<ActionResult> SearchUsers([FromQuery] string query)
        {
            _logger.LogInformation($"🔍 Recherche utilisateurs avec query: {query}");

            if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            {
                _logger.LogWarning("❌ Requête de recherche invalide");
                return BadRequest("La requête doit contenir au moins 2 caractères");
            }

            try
            {
                var utilisateurs = await _context.Utilisateurs
                    .Where(u =>
                        u.Nom.ToLower().Contains(query.ToLower()) ||
                        u.Prenom.ToLower().Contains(query.ToLower()) ||
                        u.Email.ToLower().Contains(query.ToLower())
                    )
                    .Select(u => new {
                        u.Id,
                        u.Email,
                        u.Nom,
                        u.Prenom,
                        FullName = $"{u.Prenom} {u.Nom}",
                        DisplayText = $"{u.Prenom} {u.Nom} ({u.Email})",
                        Role = u.Role.ToString()
                    })
                    .Take(20)
                    .ToListAsync();

                _logger.LogInformation($"✅ {utilisateurs.Count} utilisateurs trouvés");
                return Ok(utilisateurs);
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Erreur lors de la recherche: {ex.Message}");
                return StatusCode(500, "Erreur lors de la recherche des utilisateurs");
            }
        }

        [HttpGet("{id}/basic")]
        [Authorize]
        public async Task<ActionResult> GetUserBasicInfo(int id)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            _logger.LogInformation($"👤 Accès aux infos basiques - ID demandé: {id}, User: {currentUserId}, Role: {userRole}");

            // Debug des claims
            foreach (var claim in User.Claims)
            {
                _logger.LogInformation($"🔍 Claim: {claim.Type} = {claim.Value}");
            }

            if (!IsCurrentUserAdmin() && currentUserId != id.ToString())
            {
                _logger.LogWarning($"🚫 Accès non autorisé aux infos de {id}");
                return Forbid();
            }

            try
            {
                var utilisateur = await _context.Utilisateurs
                    .Where(u => u.Id == id)
                    .Select(u => new {
                        u.Id,
                        u.Email,
                        u.Nom,
                        u.Prenom,
                        FullName = $"{u.Prenom} {u.Nom}",
                        u.ProfilePhotoUrl,
                        Role = u.Role.ToString()
                    })
                    .FirstOrDefaultAsync();

                if (utilisateur == null)
                {
                    _logger.LogWarning($"❌ Utilisateur non trouvé: {id}");
                    return NotFound();
                }

                _logger.LogInformation($"✅ Infos basiques récupérées pour: {id}");
                return Ok(utilisateur);
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Erreur lors de la récupération des infos: {ex.Message}");
                return StatusCode(500, "Erreur lors de la récupération des informations");
            }
        }

        [HttpPost("upload-photo")]
        [Authorize]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadPhoto([FromForm] UploadPhotoDto dto)
        {
            _logger.LogInformation("=== DÉBUT UPLOAD PHOTO ===");

            int userId;
            if (dto.UserId.HasValue && dto.UserId.Value > 0)
            {
                userId = dto.UserId.Value;
                _logger.LogInformation($"📝 UserId from FormData: {userId}");
            }
            else
            {
                _logger.LogInformation("🔍 Recherche UserId dans les claims");
                var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!int.TryParse(userIdString, out userId))
                {
                    _logger.LogError("❌ UserId invalide");
                    return BadRequest("ID utilisateur invalide");
                }
            }

            try
            {
                var command = new UploadProfilePhotoCommand
                {
                    UserId = userId,
                    Photo = dto.Photo
                };

                var photoUrl = await _mediator.Send(command);
                _logger.LogInformation($"✅ Photo uploadée: {photoUrl}");
                return Ok(new { PhotoUrl = photoUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Erreur upload: {ex.Message}");
                return BadRequest($"Erreur lors de l'upload: {ex.Message}");
            }
        }
        [HttpGet("for-messaging")]
        [Authorize] // Tous les utilisateurs connectés peuvent l'utiliser
        public async Task<ActionResult> GetUsersForMessaging()
        {
            _logger.LogInformation("📧 Récupération des utilisateurs pour messagerie");

            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            _logger.LogInformation($"🔍 Utilisateur courant: {currentUserId}");

            try
            {
                var utilisateurs = await _context.Utilisateurs
                    .Where(u => u.Id.ToString() != currentUserId) // Exclure l'utilisateur courant
                    .Select(u => new {
                        u.Id,
                        u.Email,
                        u.Nom,
                        u.Prenom,
                        FullName = $"{u.Prenom} {u.Nom}",
                        Role = u.Role.ToString()
                    })
                    .OrderBy(u => u.Nom)
                    .ThenBy(u => u.Prenom)
                    .ToListAsync();

                _logger.LogInformation($"✅ {utilisateurs.Count} utilisateurs récupérés pour messagerie");
                return Ok(utilisateurs);
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Erreur lors de la récupération: {ex.Message}");
                return StatusCode(500, "Erreur lors de la récupération des utilisateurs");
            }
        }

        [HttpGet("image/{fileName}")]
        public IActionResult GetProfileImage(string fileName)
        {
            _logger.LogInformation($"🔍 Récupération image: {fileName}");

            try
            {
                var baseUploadPath = _environment?.WebRootPath ??
                                    Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

                var filePath = Path.Combine(baseUploadPath, "profile-photos", fileName);
                var folderPath = Path.Combine(baseUploadPath, "profile-photos");

                if (!Directory.Exists(folderPath))
                {
                    _logger.LogError($"❌ Dossier non trouvé: {folderPath}");
                    return NotFound("Dossier profile-photos non trouvé");
                }

                if (!System.IO.File.Exists(filePath))
                {
                    _logger.LogWarning($"❌ Image non trouvée: {fileName}");
                    return NotFound($"Image {fileName} non trouvée");
                }

                var fileBytes = System.IO.File.ReadAllBytes(filePath);
                var extension = Path.GetExtension(fileName).ToLowerInvariant();

                var contentType = extension switch
                {
                    ".jpg" or ".jpeg" => "image/jpeg",
                    ".png" => "image/png",
                    ".gif" => "image/gif",
                    ".webp" => "image/webp",
                    _ => "application/octet-stream"
                };

                _logger.LogInformation($"✅ Image servie: {fileName}, {fileBytes.Length} bytes");
                return File(fileBytes, contentType);
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Erreur: {ex.Message}");
                return StatusCode(500, $"Erreur lors de la récupération de l'image: {ex.Message}");
            }
        }

    }
}