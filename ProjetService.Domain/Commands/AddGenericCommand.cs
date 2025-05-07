using MediatR;

namespace ProjetService.Domain.Commands
{
    public class AddGenericCommand<T> : IRequest<T> where T : class
    {
        public T Entity { get; }

        public AddGenericCommand(T entity)
        {
            Entity = entity;
        }
    }
}
