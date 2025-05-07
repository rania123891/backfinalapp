using MediatR;
using ProjetService.Domain.Interfaces;

namespace ProjetService.Domain.Queries
{
    public class GetGenericQuery<T> : IRequest<T> where T : class
    {
        public int Id { get; }

        public GetGenericQuery(int id)
        {
            Id = id;
        }
    }
}
