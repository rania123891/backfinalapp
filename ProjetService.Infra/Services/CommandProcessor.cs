using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ProjetService.Domain.Models;
using ProjetService.Domain.Interfaces;
using ProjetService.Domain.Interface;
using ProjetService.Domain.Commands;
using MediatR;

namespace ProjetService.Infra.Services
{
    public class CommandProcessor : ICommandProcessor
    {
        private readonly IMediator _mediator;

        public CommandProcessor(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<CommandResult> ProcessAsync(string command)
        {
            try
            {
                var commandLower = command.ToLower().Trim();

                // Détecter le type de commande
                if (IsPlanificationCommand(commandLower))
                {
                    return await ProcessPlanificationCommand(command);
                }
                else if (IsProjetCommand(commandLower))
                {
                    return await ProcessProjetCommand(command);
                }
                else if (IsTacheCommand(commandLower))
                {
                    return await ProcessTacheCommand(command);
                }
                else
                {
                    return new CommandResult
                    {
                        Success = false,
                        Message = "Je n'ai pas compris votre commande. Essayez quelque chose comme:\n" +
                                 "- 'J'ai travaillé sur le projet X pour la tâche Y de 8h à 9h'\n" +
                                 "- 'Créer un projet nommé X'\n" +
                                 "- 'Ajouter une tâche Y au projet X'",
                        Type = CommandType.General
                    };
                }
            }
            catch (Exception ex)
            {
                return new CommandResult
                {
                    Success = false,
                    Message = $"Erreur lors du traitement de la commande: {ex.Message}",
                    Type = CommandType.General
                };
            }
        }

        private bool IsPlanificationCommand(string command)
        {
            var planificationKeywords = new[]
            {
                "j'ai fait", "j'ai travaillé", "j'ai bossé", "travail", "planification",
                "de", "à", "heure", "h", "description", "terminé", "fini", "en cours", "progress"
            };

            return Array.Exists(planificationKeywords, keyword => command.Contains(keyword));
        }

        private bool IsProjetCommand(string command)
        {
            var projetKeywords = new[] { "créer un projet", "nouveau projet", "ajouter un projet", "projet nommé" };
            return Array.Exists(projetKeywords, keyword => command.Contains(keyword));
        }

        private bool IsTacheCommand(string command)
        {
            var tacheKeywords = new[] { "créer une tâche", "nouvelle tâche", "ajouter une tâche", "tâche nommée" };
            return Array.Exists(tacheKeywords, keyword => command.Contains(keyword));
        }

        private async Task<CommandResult> ProcessPlanificationCommand(string command)
        {
            // Pour les planifications, on fait appel au frontend via le service NLP
            // Car il a accès aux APIs et peut faire l'extraction intelligente
            return new CommandResult
            {
                Success = false,
                Message = "Les commandes de planification sont traitées côté frontend pour une meilleure intelligence.",
                Type = CommandType.Planification
            };
        }

        private async Task<CommandResult> ProcessProjetCommand(string command)
        {
            try
            {
                // Extraire le nom du projet
                var match = Regex.Match(command, @"projet nommé ['""]([^'""]+)['""]", RegexOptions.IgnoreCase);
                if (!match.Success)
                {
                    match = Regex.Match(command, @"projet ([a-zA-Z0-9àâäéèêëïîôöùûüÿç\s\-_]+)", RegexOptions.IgnoreCase);
                }

                if (match.Success)
                {
                    var projetNom = match.Groups[1].Value.Trim();

                    // Extraire la description si présente
                    var descMatch = Regex.Match(command, @"qui vise à ['""]([^'""]+)['""]", RegexOptions.IgnoreCase);
                    var description = descMatch.Success ? descMatch.Groups[1].Value : "Créé via assistant vocal";

                    // Extraire la date d'échéance si présente
                    var dateMatch = Regex.Match(command, @"échéance le (\d{1,2}\s+\w+\s+\d{4})", RegexOptions.IgnoreCase);
                    DateTime? dateEcheance = null;
                    if (dateMatch.Success && DateTime.TryParse(dateMatch.Groups[1].Value, out var parsedDate))
                    {
                        dateEcheance = parsedDate;
                    }

                    // Créer la commande pour créer le projet
                    var createCommand = new CreateProjetCommand
                    {
                        Nom = projetNom,
                        Description = description,
                        DateDebut = DateTime.Now,
                        DateEcheance = dateEcheance ?? DateTime.Now.AddMonths(3),
                        Statut = StatutProjet.EnCours,
                        CreateurId = 1
                    };

                    var result = await _mediator.Send(createCommand);

                    return new CommandResult
                    {
                        Success = true,
                        Message = $"✅ Projet '{projetNom}' créé avec succès!\n📝 Description: {description}",
                        Data = result,
                        Type = CommandType.Projet
                    };
                }
                else
                {
                    return new CommandResult
                    {
                        Success = false,
                        Message = "Je n'ai pas pu identifier le nom du projet. Essayez: 'Créer un projet nommé \"Mon Projet\"'",
                        Type = CommandType.Projet
                    };
                }
            }
            catch (Exception ex)
            {
                return new CommandResult
                {
                    Success = false,
                    Message = $"Erreur lors de la création du projet: {ex.Message}",
                    Type = CommandType.Projet
                };
            }
        }

        private async Task<CommandResult> ProcessTacheCommand(string command)
        {
            // Implémentation similaire pour les tâches
            return new CommandResult
            {
                Success = false,
                Message = "Fonctionnalité de création de tâches en cours de développement",
                Type = CommandType.Tache
            };
        }
    }
}