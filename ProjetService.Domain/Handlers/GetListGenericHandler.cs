using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjetService.Domain.Queries;
using ProjetService.Domain.Interfaces;

namespace ProjetService.Domain.Handlers
{
    public class GetListGenericHandler<T> : IRequestHandler<GetListGenericQuery<T>, IEnumerable<T>> where T : class
    {
        private readonly IGenericRepository<T> _repository;

        public GetListGenericHandler(IGenericRepository<T> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<T>> Handle(GetListGenericQuery<T> request, CancellationToken cancellationToken)
        {
            return await _repository.GetAllAsync();
        }
    }
}
