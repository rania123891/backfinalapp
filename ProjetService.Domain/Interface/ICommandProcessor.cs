using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetService.Domain.Interface
{
    public interface ICommandProcessor
    {
        Task<string> ProcessAsync(string command);
    }
}
