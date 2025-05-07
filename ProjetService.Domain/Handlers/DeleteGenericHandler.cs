using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjetService.Domain.Commands;
using ProjetService.Domain.Interfaces;

namespace ProjetService.Domain.Handlers
{
    public class DeleteGenericHandler<T> : IRequestHandler<DeleteGenericCommand<T>, Unit> where T : class
    {
        private readonly IGenericRepository<T> _repository;

        public DeleteGenericHandler(IGenericRepository<T> repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(DeleteGenericCommand<T> request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.Id);
            if (entity == null)
            {
                throw new KeyNotFoundException($"{typeof(T).Name} avec ID {request.Id} non trouvé.");
            }

            await _repository.DeleteAsync(entity);
            return Unit.Value; // ✅ Retourne MediatR.Unit
        }
    }
}
