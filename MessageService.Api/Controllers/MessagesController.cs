using MediatR;
using Microsoft.AspNetCore.Mvc;
using MessageService.Domain.Commands;
using MessageService.Domain.Models;
using MessageService.Domain.Queries;
using MessageService.Domain.DTOs;
using System.Net.Http;
using System.Text.Json;
using MessageService.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MessageService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessagesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMessageRepository _messageRepository;
    private readonly HttpClient _httpClient;

    public MessagesController(IMediator mediator, IMessageRepository messageRepository, IHttpClientFactory httpClientFactory)
    {
        _mediator = mediator;
        _messageRepository = messageRepository;
        _httpClient = httpClientFactory.CreateClient();
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<IEnumerable<Message>>> GetMessagesByUser(string userId)
    {
        try
        {
            // Récupérer tous les messages où l'utilisateur est soit expéditeur soit destinataire
            var messages = await _messageRepository.GetMessagesByUserAsync(userId);
            return Ok(messages);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Une erreur est survenue lors de la récupération des messages: {ex.Message}");
        }
    }


    [HttpPost("send")]
    public async Task<IActionResult> EnvoyerMessage([FromBody] CreateMessageDto dto)
    {
        // Récupérer expéditeur par email
        var responseExpediteur = await _httpClient.GetAsync($"http://localhost:5093/user/api/Utilisateur/by-email/{dto.ExpediteurEmail}");
        if (!responseExpediteur.IsSuccessStatusCode)
            return NotFound("Expéditeur introuvable");
        var contentExp = await responseExpediteur.Content.ReadAsStringAsync();
        var expediteur = JsonSerializer.Deserialize<UtilisateurDto>(contentExp, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        if (expediteur == null)
            return NotFound("Expéditeur non trouvé");

        // Récupérer destinataire par email
        var responseDestinataire = await _httpClient.GetAsync($"http://localhost:5093/user/api/Utilisateur/by-email/{dto.EmailDestinataire}");
        if (!responseDestinataire.IsSuccessStatusCode)
            return NotFound("Destinataire introuvable");
        var contentDest = await responseDestinataire.Content.ReadAsStringAsync();
        var destinataire = JsonSerializer.Deserialize<UtilisateurDto>(contentDest, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        if (destinataire == null)
            return NotFound("Destinataire non trouvé");

        var message = new Message
        {
            Contenu = dto.Contenu,
            ExpediteurId = expediteur.Id.ToString(),
            DestinataireId = destinataire.Id.ToString(),
            EnvoyeLe = DateTime.UtcNow
        };

        await _messageRepository.AddAsync(message);
        return Created("", message.Id);
    }


}
