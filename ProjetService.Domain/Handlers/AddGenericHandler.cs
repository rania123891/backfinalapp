using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjetService.Domain.Commands;
using ProjetService.Domain.Interfaces;

namespace ProjetService.Domain.Handlers
{
    public class AddGenericHandler<T> : IRequestHandler<AddGenericCommand<T>, T> where T : class
    {
        private readonly IGenericRepository<T> _repository;

        public AddGenericHandler(IGenericRepository<T> repository)
        {
            _repository = repository;
        }

        public async Task<T> Handle(AddGenericCommand<T> request, CancellationToken cancellationToken)
        {
            await _repository.AddAsync(request.Entity);
            return request.Entity;
        }
    }
}
