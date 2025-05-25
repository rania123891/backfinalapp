// MessageService.Domain/Queries/GetMessagesByUserHandler.cs
using MediatR;
using MessageService.Data.Repositories;
using MessageService.Domain.DTOs;
using MessageService.Domain.Queries;
using System.Text.Json;

public class GetMessagesByUserHandler : IRequestHandler<GetMessagesByUserQuery, List<MessageDto>>
{
    private readonly IMessageRepository _repository;
    private readonly HttpClient _httpClient;

    public GetMessagesByUserHandler(IMessageRepository repository)
    {
        _repository = repository;
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:5093/")
        };
    }

    public async Task<List<MessageDto>> Handle(GetMessagesByUserQuery request, CancellationToken cancellationToken)
    {
        var messages = await _repository.GetMessagesByUserAsync(request.UserId);
        var messagesDtos = new List<MessageDto>();

        foreach (var message in messages)
        {
            var messageDto = new MessageDto
            {
                Id = message.Id,
                Contenu = message.Contenu,
                ExpediteurId = message.ExpediteurId,
                DestinataireId = message.DestinataireId,
                EnvoyeLe = message.EnvoyeLe
            };

            try
            {
                // Convertir l'ID en entier pour l'API utilisateur
                if (int.TryParse(message.ExpediteurId, out int expediteurId))
                {
                    var responseExp = await _httpClient.GetAsync($"user/api/Utilisateur/{expediteurId}", cancellationToken);
                    if (responseExp.IsSuccessStatusCode)
                    {
                        var contentExp = await responseExp.Content.ReadAsStringAsync(cancellationToken);
                        messageDto.Expediteur = JsonSerializer.Deserialize<UtilisateurDto>(contentExp,
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    }
                }

                if (int.TryParse(message.DestinataireId, out int destinataireId))
                {
                    var responseDest = await _httpClient.GetAsync($"user/api/Utilisateur/{destinataireId}", cancellationToken);
                    if (responseDest.IsSuccessStatusCode)
                    {
                        var contentDest = await responseDest.Content.ReadAsStringAsync(cancellationToken);
                        messageDto.Destinataire = JsonSerializer.Deserialize<UtilisateurDto>(contentDest,
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la récupération des détails utilisateur: {ex.Message}");
            }

            messagesDtos.Add(messageDto);
        }

        return messagesDtos;
    }
}