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



        public UtilisateurController(IMediator mediator, IUtilisateurRepository repository,
            IAuthService authService, UserDbContext context, IWebHostEnvironment environment)
        {
            _mediator = mediator;
            _repository = repository;
            _authService = authService;
            _context = context;
            _environment = environment;

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

        [HttpGet("{id}")]
        public async Task<ActionResult<UtilisateurDto>> GetUtilisateur(int id)
        {
            var utilisateur = await _context.Utilisateurs.FindAsync(id);
            if (utilisateur == null)
                return NotFound();

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
        public async Task<ActionResult<UtilisateurDto>> GetByEmail(string email)
        {
            var utilisateur = await _context.Utilisateurs
                .FirstOrDefaultAsync(u => u.Email == email);

            if (utilisateur == null)
                return NotFound();

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

            return Ok(dtos);
        }
        [HttpPost("upload-photo")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadPhoto([FromForm] UploadPhotoDto dto)
        {
            Console.WriteLine("=== DÉBUT DEBUG UPLOAD ===");

            // 🚧 Temporaire : utiliser l'UserId du FormData si disponible
            int userId;

            if (dto.UserId.HasValue && dto.UserId.Value > 0)
            {
                userId = dto.UserId.Value;
                Console.WriteLine($"🚧 Utilisation UserId du FormData: {userId}");
            }
            else
            {
                // Fallback : essayer de récupérer depuis les claims JWT
                Console.WriteLine("🔍 Tentative de récupération depuis JWT claims...");

                // Debug: afficher tous les claims
                foreach (var claim in User.Claims)
                {
                    Console.WriteLine($"Claim: {claim.Type} = {claim.Value}");
                }

                var userIdString = User.FindFirst("nameid")?.Value;
                Console.WriteLine($"nameid trouvé: {userIdString}");

                if (!int.TryParse(userIdString, out userId))
                {
                    Console.WriteLine("❌ Impossible de récupérer l'ID utilisateur");
                    return BadRequest($"ID utilisateur invalide. UserId FormData: {dto.UserId}, nameid claim: {userIdString}");
                }
            }

            Console.WriteLine($"✅ UserId final utilisé: {userId}");

            try
            {
                var command = new UploadProfilePhotoCommand
                {
                    UserId = userId,
                    Photo = dto.Photo
                };

                var photoUrl = await _mediator.Send(command);
                Console.WriteLine($"✅ Photo uploadée avec succès: {photoUrl}");

                return Ok(new { PhotoUrl = photoUrl });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur lors de l'upload: {ex.Message}");
                return BadRequest($"Erreur lors de l'upload: {ex.Message}");
            }
            finally
            {
                Console.WriteLine("=== FIN DEBUG UPLOAD ===");
            }
        }
        [HttpGet("image/{fileName}")]
        public IActionResult GetProfileImage(string fileName)
        {
            Console.WriteLine($"🔍 GetProfileImage appelé avec fileName: {fileName}");

            try
            {
                // Récupérer le chemin de base
                var baseUploadPath = _environment?.WebRootPath ??
                                    Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

                Console.WriteLine($"💾 Base path: {baseUploadPath}");

                var filePath = Path.Combine(baseUploadPath, "profile-photos", fileName);
                Console.WriteLine($"📁 Looking for file: {filePath}");

                // Vérifier si le dossier existe
                var folderPath = Path.Combine(baseUploadPath, "profile-photos");
                if (!Directory.Exists(folderPath))
                {
                    Console.WriteLine($"❌ Dossier n'existe pas: {folderPath}");
                    return NotFound($"Dossier profile-photos n'existe pas");
                }

                // Lister tous les fichiers dans le dossier
                var files = Directory.GetFiles(folderPath);
                Console.WriteLine($"📂 Fichiers dans le dossier ({files.Length}):");
                foreach (var file in files)
                {
                    Console.WriteLine($"  - {Path.GetFileName(file)}");
                }

                if (!System.IO.File.Exists(filePath))
                {
                    Console.WriteLine($"❌ Fichier non trouvé: {filePath}");
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

                Console.WriteLine($"✅ Fichier servi: {fileName}, taille: {fileBytes.Length} bytes, type: {contentType}");
                return File(fileBytes, contentType);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur: {ex.Message}");
                Console.WriteLine($"❌ Stack trace: {ex.StackTrace}");
                return BadRequest($"Erreur: {ex.Message}");
            }
        }


    }



}
