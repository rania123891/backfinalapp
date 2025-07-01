using MediatR;
using Microsoft.AspNetCore.Mvc;
using MessageService.Domain.Commands;
using MessageService.Domain.Models;
using MessageService.Domain.Queries;
using MessageService.Domain.DTOs;
using System.Net.Http;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MessageService.Domain.Interfaces;

namespace MessageService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessagesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMessageRepository _messageRepository;
    private readonly HttpClient _httpClient;
    private readonly ILogger<MessagesController> _logger;       
    private readonly IFileService _fileService;

    public MessagesController(IMediator mediator, IMessageRepository messageRepository, IHttpClientFactory httpClientFactory , ILogger<MessagesController> logger, IFileService fileService)  
    {
        _mediator = mediator;
        _messageRepository = messageRepository;
        _httpClient = httpClientFactory.CreateClient();
        _logger = logger;                                      
        _fileService = fileService;
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<IEnumerable<Message>>> GetMessagesByUser(string userId)
    {
        try
        {
            _logger.LogInformation($"🔍 Récupération des messages pour l'utilisateur {userId}");
            
            // Récupérer tous les messages où l'utilisateur est soit expéditeur soit destinataire
            var messages = await _messageRepository.GetMessagesByUserAsync(userId);
            
            _logger.LogInformation($"📊 {messages.Count} messages trouvés pour l'utilisateur {userId}");
            return Ok(messages);
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ Erreur lors de la récupération des messages pour {userId}: {ex.Message}");
            return StatusCode(500, $"Une erreur est survenue lors de la récupération des messages: {ex.Message}");
        }
    }

    [HttpGet("message/{messageId:guid}")]
    public async Task<ActionResult<Message>> GetMessageById(Guid messageId)
    {
        try
        {
            _logger.LogInformation($"🔍 Récupération du message {messageId}");
            
            // Note: Vous devrez ajouter cette méthode dans IMessageRepository
            var message = await _messageRepository.GetByIdAsync(messageId);
            
            if (message == null)
            {
                _logger.LogWarning($"❌ Message {messageId} non trouvé");
                return NotFound($"Message {messageId} non trouvé");
            }
            
            _logger.LogInformation($"✅ Message {messageId} trouvé");
            return Ok(message);
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ Erreur lors de la récupération du message {messageId}: {ex.Message}");
            return StatusCode(500, $"Une erreur est survenue lors de la récupération du message: {ex.Message}");
        }
    }

    [HttpPost("send")]
    public async Task<IActionResult> EnvoyerMessage([FromBody] CreateMessageDto dto)
    {
        string expediteurId;
        string destinataireId;

        // Si les IDs sont fournis, les utiliser directement (plus efficace)
        if (!string.IsNullOrEmpty(dto.ExpediteurId) && !string.IsNullOrEmpty(dto.DestinataireId))
        {
            expediteurId = dto.ExpediteurId;
            destinataireId = dto.DestinataireId;

            // Vérification optionnelle que les utilisateurs existent
            var responseExp = await _httpClient.GetAsync($"http://localhost:5093/user/api/Utilisateur/{expediteurId}");
            if (!responseExp.IsSuccessStatusCode)
                return NotFound("Expéditeur introuvable");

            var responseDest = await _httpClient.GetAsync($"http://localhost:5093/user/api/Utilisateur/{destinataireId}");
            if (!responseDest.IsSuccessStatusCode)
                return NotFound("Destinataire introuvable");
        }
        else
        {
            // Fallback: chercher par email (nécessite authentification)
            // TODO: Configurer HttpClient avec token ou créer endpoint interne
            return BadRequest("IDs utilisateurs requis pour l'envoi de messages");
        }

        var message = new Message
        {
            Contenu = dto.Contenu,
            ExpediteurId = expediteurId,
            DestinataireId = destinataireId,
            EnvoyeLe = DateTime.UtcNow
        };

        await _messageRepository.AddAsync(message);
        return Created("", message.Id);
    }
    [HttpPost("simple-send")]
    public async Task<IActionResult> SimpleSendMessage([FromBody] CreateMessageDto dto)
    {
        try
        {
            // Utiliser directement les IDs fournis (pas de vérification pour le test)
            var message = new Message
            {
                Contenu = dto.Contenu,
                ExpediteurId = dto.ExpediteurId ?? "1", // Default fallback
                DestinataireId = dto.DestinataireId ?? "5", // Default fallback  
                EnvoyeLe = DateTime.UtcNow
            };

            await _messageRepository.AddAsync(message);
            return Ok(new { success = true, messageId = message.Id });
        }
        catch (Exception ex)
        {
            return BadRequest($"Erreur: {ex.Message}");
        }
    }
    // MessageService.Api/Controllers/MessagesController.cs (mise à jour)
    [HttpPost("send-with-attachments")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> EnvoyerMessageAvecFichiers([FromForm] CreateMessageWithAttachmentsDto dto)
    {
        try
        {
            // Validation et récupération des utilisateurs (code existant)...

            var message = new Message
            {
                Contenu = dto.Contenu,
                ExpediteurId = dto.ExpediteurId,
                DestinataireId = dto.DestinataireId,
                EnvoyeLe = DateTime.UtcNow,
                Attachments = new List<MessageAttachment>()
            };

            // Traitement des pièces jointes
            if (dto.Attachments != null && dto.Attachments.Any())
            {
                foreach (var file in dto.Attachments)
                {
                    var filePath = await _fileService.SaveFileAsync(file, "message-attachments");

                    var attachment = new MessageAttachment
                    {
                        Id = Guid.NewGuid(),
                        MessageId = message.Id,
                        FileName = Path.GetFileName(filePath),
                        OriginalFileName = file.FileName,
                        FilePath = filePath,
                        FileType = _fileService.GetFileType(file.ContentType),
                        MimeType = file.ContentType,
                        FileSize = file.Length,
                        UploadedAt = DateTime.UtcNow,
                        UploadedBy = dto.ExpediteurId
                    };

                    message.Attachments.Add(attachment);
                }
            }

            await _messageRepository.AddAsync(message);
            return Created("", new { messageId = message.Id, attachmentsCount = message.Attachments.Count });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erreur lors de l'envoi du message avec fichiers: {ex.Message}");
            return BadRequest($"Erreur: {ex.Message}");
        }
    }

}