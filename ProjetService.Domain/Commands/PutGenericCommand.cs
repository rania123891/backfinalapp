using MediatR;

namespace ProjetService.Domain.Commands
{
    public class PutGenericCommand<T> : IRequest<T> where T : class
    {
        public T Entity { get; }

        public PutGenericCommand(T entity)
        {
            Entity = entity;
        }
    }
}
