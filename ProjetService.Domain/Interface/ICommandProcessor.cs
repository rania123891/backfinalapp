using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetService.Domain.Interface
{
   
    public interface ICommandProcessor
    {
        Task<CommandResult> ProcessAsync(string command);
    }

    public class CommandResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
        public CommandType Type { get; set; }
    }

    public enum CommandType
    {
        Planification,
        Projet,
        Tache,
        General
    }
}
