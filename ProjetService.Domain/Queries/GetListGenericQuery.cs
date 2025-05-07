using MediatR;
using System.Collections.Generic;

namespace ProjetService.Domain.Queries
{
    public class GetListGenericQuery<T> : IRequest<IEnumerable<T>> where T : class
    {
    }
}
