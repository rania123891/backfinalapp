using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjetService.Domain.Interfaces;
using ProjetService.Domain.Queries;

namespace ProjetService.Domain.Handlers
{
    public class GetGenericHandler<T> : IRequestHandler<GetGenericQuery<T>, T> where T : class
    {
        private readonly IGenericRepository<T> _repository;

        public GetGenericHandler(IGenericRepository<T> repository)
        {
            _repository = repository;
        }

        public async Task<T> Handle(GetGenericQuery<T> request, CancellationToken cancellationToken)
        {
            return await _repository.GetByIdAsync(request.Id);
        }
    }
}
