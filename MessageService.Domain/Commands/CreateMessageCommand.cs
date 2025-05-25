using MediatR;
using MessageService.Domain.DTOs;

namespace MessageService.Domain.Commands;

// MessageService.Domain/Commands/CreateMessageCommand.cs
public class CreateMessageCommand : IRequest<Guid>
{
    public string Contenu { get; set; }
    public string ExpediteurId { get; set; }  // Changé en string
    public string DestinataireId { get; set; }
}