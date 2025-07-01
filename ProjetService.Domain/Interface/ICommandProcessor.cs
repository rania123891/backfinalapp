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
        public float Confidence { get; set; } // Niveau de confiance
        public Dictionary<string, object> ExtractedData { get; set; } = new Dictionary<string, object>();
    }

    public enum CommandType
    {
        Planification,
        Projet,
        Tache,
        Equipe,
        Membre,
        General
    }
}