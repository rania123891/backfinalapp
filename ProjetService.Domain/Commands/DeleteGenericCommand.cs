using MediatR;

namespace ProjetService.Domain.Commands
{
    public class DeleteGenericCommand<T> : IRequest<Unit> where T : class
    {
        public int Id { get; }

        public DeleteGenericCommand(int id)
        {
            Id = id;
        }
    }
}
