using MediatR;
using MessageService.Domain.Commands;
using MessageService.Domain.Interfaces;
using MessageService.Domain.Models;

namespace MessageService.Domain.Handlers;
public class CreateMessageHandler : IRequestHandler<CreateMessageCommand, Guid>
{
    private readonly IMessageRepository _repository;

    public CreateMessageHandler(IMessageRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(CreateMessageCommand request, CancellationToken cancellationToken)
    {
        var message = new Message
        {
            Id = Guid.NewGuid(),
            Contenu = request.Contenu,
            ExpediteurId = request.ExpediteurId.ToString(),  // Conversion en string
            DestinataireId = request.DestinataireId.ToString(),  // Conversion en string
            EnvoyeLe = DateTime.UtcNow
        };

        await _repository.AddAsync(message);
        return message.Id;
    }
}