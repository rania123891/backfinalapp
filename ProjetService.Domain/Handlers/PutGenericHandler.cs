using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjetService.Domain.Commands;
using ProjetService.Domain.Interfaces;

namespace ProjetService.Domain.Handlers
{
    public class PutGenericHandler<T> : IRequestHandler<PutGenericCommand<T>, T> where T : class
    {
        private readonly IGenericRepository<T> _repository;

        public PutGenericHandler(IGenericRepository<T> repository)
        {
            _repository = repository;
        }

        public async Task<T> Handle(PutGenericCommand<T> request, CancellationToken cancellationToken)
        {
            await _repository.UpdateAsync(request.Entity);
            return request.Entity;
        }
    }
}
