 using MediatR;
using MessageService.Domain.DTOs;

namespace MessageService.Domain.Queries;

public class GetMessagesByUserQuery : IRequest<List<MessageDto>>
{
    public string UserId { get; }

    public GetMessagesByUserQuery(string userId)
    {
        UserId = userId;
    }
}
