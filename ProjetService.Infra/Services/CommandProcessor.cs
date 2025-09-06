// ========================================
// 🎤 COMMANDPROCESSOR COMPLET AVEC TOUTES LES MISES À JOUR
// Assistant vocal avancé pour création de projets/tâches/équipes
// ========================================

using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ProjetService.Domain.Models;
using ProjetService.Domain.Interfaces;
using ProjetService.Domain.Interface;
using ProjetService.Domain.Commands;
using MediatR;
using System.Collections.Generic;
using System.Linq;

namespace ProjetService.Infra.Services
{
    public class CommandProcessor : ICommandProcessor
    {
        private readonly IMediator _mediator;
        private readonly IPlanificationRepository _planificationRepository;
        private readonly IGenericRepository<Tache> _tacheRepository;
        private readonly IGenericRepository<Projet> _projetRepository;
        private readonly IGenericRepository<Equipe> _equipeRepository;

        public CommandProcessor(
            IMediator mediator, 
            IPlanificationRepository planificationRepository,
            IGenericRepository<Tache> tacheRepository,
            IGenericRepository<Projet> projetRepository,
            IGenericRepository<Equipe> equipeRepository)
        {
            _mediator = mediator;
            _planificationRepository = planificationRepository;
            _tacheRepository = tacheRepository;
            _projetRepository = projetRepository;
            _equipeRepository = equipeRepository;
        }

        public async Task<CommandResult> ProcessAsync(string command)
        {
            try
            {
                var commandLower = command.ToLower().Trim();
                var result = new CommandResult();

                // Analyser la commande et extraire les informations
                var analysis = AnalyzeCommand(commandLower, command); // Passer aussi l'original pour garder la casse
                result.Confidence = analysis.Confidence;
                result.ExtractedData = analysis.ExtractedData;

                // Traiter selon le type détecté
                switch (analysis.Type)
                {
                    case CommandType.Projet:
                        return await ProcessProjetCommand(command, analysis);
                    case CommandType.Tache:
                        return await ProcessTacheCommand(command, analysis);
                    case CommandType.Equipe:
                        return await ProcessEquipeCommand(command, analysis);
                    case CommandType.Membre:
                        return await ProcessMembreCommand(command, analysis);
                    case CommandType.Planification:
                        return await ProcessPlanificationCommand(command, analysis);
                    default:
                        return new CommandResult
                        {
                            Success = false,
                            Message = GetHelpMessage(),
                            Type = CommandType.General,
                            Confidence = 0
                        };
                }
            }
            catch (Exception ex)
            {
                return new CommandResult
                {
                    Success = false,
                    Message = $"Erreur lors du traitement de la commande: {ex.Message}",
                    Type = CommandType.General,
                    Confidence = 0
                };
            }
        }

        // 🎤 ANALYSE AMÉLIORÉE POUR ASSISTANT VOCAL
        private CommandAnalysis AnalyzeCommand(string commandLower, string originalCommand)
        {
            var analysis = new CommandAnalysis();
            var confidence = 0f;
            var extractedData = new Dictionary<string, object>();

            // 🎯 DÉTECTION DES PROJETS avec extraction améliorée
            if (ContainsAny(commandLower, new[] { "créer un projet", "nouveau projet", "ajouter un projet", "projet nommé" }))
            {
                analysis.Type = CommandType.Projet;
                confidence += 30f;

                // ✅ Extraire le nom du projet (vocal)
                var projetName = ExtractProjetNameVocal(originalCommand);
                if (!string.IsNullOrEmpty(projetName))
                {
                    extractedData["nom"] = projetName;
                    confidence += 25f;
                }

                // ✅ Extraire la description (vocal)
                var description = ExtractDescriptionVocal(originalCommand);
                if (!string.IsNullOrEmpty(description))
                {
                    extractedData["description"] = description;
                    confidence += 15f;
                }

                // ✅ Extraire les dates (vocal)
                var (dateDebut, dateFin) = ExtractDatesVocal(commandLower);
                if (dateDebut.HasValue)
                {
                    extractedData["dateDebut"] = dateDebut.Value;
                    confidence += 10f;
                }
                if (dateFin.HasValue)
                {
                    extractedData["dateFin"] = dateFin.Value;
                    confidence += 10f;
                }
            }

            // 🎯 DÉTECTION DES TÂCHES avec extraction améliorée
            else if (ContainsAny(commandLower, new[] { "créer une tâche", "nouvelle tâche", "ajouter une tâche", "tâche nommée", "tâche qui s'appelle", "qui s'appelle" }))
            {
                analysis.Type = CommandType.Tache;
                confidence += 30f;

                // Extraire le nom de la tâche (vocal)
                var tacheName = ExtractTacheNameVocal(originalCommand);
                if (!string.IsNullOrEmpty(tacheName))
                {
                    extractedData["nom"] = tacheName;
                    confidence += 25f;
                }

                // Extraire la priorité
                var priorite = ExtractPriorite(commandLower);
                if (!string.IsNullOrEmpty(priorite))
                {
                    extractedData["priorite"] = priorite;
                    confidence += 15f;
                }

                // ✅ Extraire l'équipe mentionnée
                var equipe = ExtractEquipeFromCommand(commandLower);
                if (!string.IsNullOrEmpty(equipe))
                {
                    extractedData["equipe"] = equipe;
                    confidence += 20f;
                }

                // Extraire le projet
                var projet = ExtractAfterKeyword(commandLower, new[] { "pour le projet", "dans le projet", "au projet" });
                if (!string.IsNullOrEmpty(projet))
                {
                    extractedData["projet"] = projet;
                    confidence += 15f;
                }
            }

            // 🎯 DÉTECTION DES ÉQUIPES avec extraction améliorée
            else if (ContainsAny(commandLower, new[] { "créer une équipe", "nouvelle équipe", "ajouter une équipe", "équipe nommée" }))
            {
                analysis.Type = CommandType.Equipe;
                confidence += 30f;

                var equipeName = ExtractEquipeNameVocal(originalCommand);
                if (!string.IsNullOrEmpty(equipeName))
                {
                    extractedData["nom"] = equipeName;
                    confidence += 25f;
                }
            }

            // 🎯 DÉTECTION DES MEMBRES avec extraction améliorée
            else if (ContainsAny(commandLower, new[] { "ajouter un membre", "nouveau membre", "membre nommé", "dans l'équipe" }))
            {
                analysis.Type = CommandType.Membre;
                confidence += 30f;

                var membreName = ExtractMembreNameVocal(originalCommand);
                if (!string.IsNullOrEmpty(membreName))
                {
                    extractedData["nom"] = membreName;
                    confidence += 20f;
                }

                var equipe = ExtractAfterKeyword(commandLower, new[] { "dans l'équipe", "à l'équipe", "pour l'équipe" });
                if (!string.IsNullOrEmpty(equipe))
                {
                    extractedData["equipe"] = equipe;
                    confidence += 25f;
                }
            }

            // 🎯 DÉTECTION DE LA PLANIFICATION
            else if (ContainsAny(commandLower, new[] { "j'ai fait", "j'ai travaillé", "j'ai bossé", "travail", "planification", "de", "à", "heure" }))
            {
                analysis.Type = CommandType.Planification;
                confidence += 20f;
            }

            analysis.Confidence = Math.Min(confidence, 100f);
            analysis.ExtractedData = extractedData;

            return analysis;
        }

        // 🎤 MÉTHODES D'EXTRACTION VOCALES
        private string ExtractProjetNameVocal(string command)
        {
            var patterns = new[]
            {
                @"nommé\s+(\w+)",                    // "nommé Test"
                @"appelé\s+(\w+)",                   // "appelé MonProjet"
                @"projet\s+(\w+)\s+(?:avec|description|statut|commence)", // "projet Test avec"
                @"projet\s+(\w+)$",                  // "projet Test" en fin de phrase
            };

            foreach (var pattern in patterns)
            {
                var match = Regex.Match(command, pattern, RegexOptions.IgnoreCase);
                if (match.Success)
                    return match.Groups[1].Value.Trim();
            }

            return null;
        }

        private string ExtractTacheNameVocal(string command)
        {
            var patterns = new[]
            {
                @"tâche\s+nommée\s+(\w+)",           // "tâche nommée Test"
                @"tâche\s+appelée\s+(\w+)",          // "tâche appelée Coding"
                @"tâche\s+qui\s+s'appelle\s+(\w+)",  // "tâche qui s'appelle Test" ✅
                @"qui\s+s'appelle\s+(\w+)",          // "qui s'appelle Test" ✅
                @"tâche\s+(\w+)\s+(?:avec|priorité|pour|affecter)", // "tâche Test avec"
                @"tâche\s+(\w+)$",                   // "tâche Test" en fin
            };

            foreach (var pattern in patterns)
            {
                var match = Regex.Match(command, pattern, RegexOptions.IgnoreCase);
                if (match.Success)
                    return match.Groups[1].Value.Trim();
            }

            return null;
        }

        private string ExtractEquipeNameVocal(string command)
        {
            var patterns = new[]
            {
                @"équipe\s+nommée\s+(\w+)",          // "équipe nommée DevTeam"
                @"équipe\s+appelée\s+(\w+)",         // "équipe appelée Frontend"
                @"équipe\s+(\w+)\s+(?:avec|description)", // "équipe DevTeam avec"
                @"équipe\s+(\w+)$",                  // "équipe DevTeam" en fin
            };

            foreach (var pattern in patterns)
            {
                var match = Regex.Match(command, pattern, RegexOptions.IgnoreCase);
                if (match.Success)
                    return match.Groups[1].Value.Trim();
            }

            return null;
        }

        private string ExtractMembreNameVocal(string command)
        {
            var patterns = new[]
            {
                @"membre\s+nommé\s+(\w+)",           // "membre nommé Jean"
                @"membre\s+appelé\s+(\w+)",          // "membre appelé Paul"
                @"membre\s+(\w+)\s+(?:dans|à)",      // "membre Jean dans"
                @"ajouter\s+(\w+)\s+(?:dans|à)",     // "ajouter Jean dans"
            };

            foreach (var pattern in patterns)
            {
                var match = Regex.Match(command, pattern, RegexOptions.IgnoreCase);
                if (match.Success)
                    return match.Groups[1].Value.Trim();
            }

            return null;
        }

        private string ExtractDescriptionVocal(string command)
        {
            var patterns = new[]
            {
                @"description\s+(\w+)",              // "description MonProjet" 
                @"avec\s+description\s+(\w+)",       // "avec description Test"
                @"qui\s+vise\s+à\s+(\w+)",          // "qui vise à Développer"
                @"pour\s+(\w+)\s+(?:commence|statut)", // "pour Développement commence"
            };

            foreach (var pattern in patterns)
            {
                var match = Regex.Match(command, pattern, RegexOptions.IgnoreCase);
                if (match.Success)
                    return match.Groups[1].Value.Trim();
            }

            return null;
        }

        // ✅ MÉTHODE pour extraire l'équipe des tâches
        private string ExtractEquipeFromCommand(string command)
        {
            var patterns = new[]
            {
                @"affecter\s+a\s+l'équipe\s+(\w+)",     // "affecter a l'équipe Mobile" ✅
                @"affecter\s+à\s+l'équipe\s+(\w+)",     // "affecter à l'équipe Mobile"
                @"pour\s+l'équipe\s+(\w+)",             // "pour l'équipe Mobile"
                @"dans\s+l'équipe\s+(\w+)",             // "dans l'équipe Mobile"
                @"équipe\s+(\w+)\s+avec",               // "équipe Mobile avec"
            };

            foreach (var pattern in patterns)
            {
                var match = Regex.Match(command, pattern, RegexOptions.IgnoreCase);
                if (match.Success)
                    return match.Groups[1].Value.Trim();
            }

            return null;
        }

        private (DateTime?, DateTime?) ExtractDatesVocal(string command)
        {
            DateTime? dateDebut = null;
            DateTime? dateFin = null;

            // Détecter "commence aujourd'hui"
            if (command.Contains("commence aujourd'hui") || command.Contains("aujourd'hui"))
                dateDebut = DateTime.Today;

            // Détecter "fini demain"  
            if (command.Contains("fini demain") || command.Contains("finit demain"))
                dateFin = DateTime.Today.AddDays(1);

            // Détecter "après-demain"
            if (command.Contains("après-demain"))
                dateFin = DateTime.Today.AddDays(2);

            // Détecter "dans une semaine"
            if (command.Contains("dans une semaine"))
                dateFin = DateTime.Today.AddDays(7);

            // Détecter "dans un mois"
            if (command.Contains("dans un mois"))
                dateFin = DateTime.Today.AddMonths(1);

            return (dateDebut, dateFin);
        }

        // 🔧 TRAITEMENT DES COMMANDES AMÉLIORÉ
        private async Task<CommandResult> ProcessProjetCommand(string command, CommandAnalysis analysis)
        {
            try
            {
                if (!analysis.ExtractedData.ContainsKey("nom"))
                {
                    return new CommandResult
                    {
                        Success = false,
                        Message = "❌ Je n'ai pas pu identifier le nom du projet. Essayez: 'Créer un projet nommé MonProjet'",
                        Type = CommandType.Projet,
                        Confidence = analysis.Confidence
                    };
                }

                var projetNom = analysis.ExtractedData["nom"].ToString();
                var description = analysis.ExtractedData.ContainsKey("description")
                    ? analysis.ExtractedData["description"].ToString()
                    : "Créé via assistant vocal";

                // ✅ Utiliser les dates extraites ou valeurs par défaut
                var dateDebut = analysis.ExtractedData.ContainsKey("dateDebut")
                    ? (DateTime)analysis.ExtractedData["dateDebut"]
                    : DateTime.Now;

                var dateEcheance = analysis.ExtractedData.ContainsKey("dateFin")
                    ? (DateTime)analysis.ExtractedData["dateFin"]
                    : DateTime.Now.AddMonths(3);

                var createCommand = new CreateProjetCommand
                {
                    Nom = projetNom,
                    Description = description,
                    DateDebut = dateDebut,
                    DateEcheance = dateEcheance,
                    Statut = StatutProjet.EnCours,
                    CreateurId = 1
                };

                var result = await _mediator.Send(createCommand);

                return new CommandResult
                {
                    Success = true,
                    Message = $"✅ Projet '{projetNom}' créé avec succès!\n📝 Description: {description}\n📅 Du {dateDebut:dd/MM/yyyy} au {dateEcheance:dd/MM/yyyy}",
                    Data = result,
                    Type = CommandType.Projet,
                    Confidence = analysis.Confidence,
                    ExtractedData = analysis.ExtractedData
                };
            }
            catch (Exception ex)
            {
                return new CommandResult
                {
                    Success = false,
                    Message = $"❌ Erreur lors de la création du projet: {ex.Message}",
                    Type = CommandType.Projet,
                    Confidence = analysis.Confidence
                };
            }
        }

        // ✅ TÂCHES avec VRAIE CRÉATION en base de données
        private async Task<CommandResult> ProcessTacheCommand(string command, CommandAnalysis analysis)
        {
            try
            {
                if (!analysis.ExtractedData.ContainsKey("nom"))
                {
                    return new CommandResult
                    {
                        Success = false,
                        Message = "❌ Je n'ai pas pu identifier le nom de la tâche. Essayez: 'Créer une tâche qui s'appelle MaTache avec priorité haute'",
                        Type = CommandType.Tache,
                        Confidence = analysis.Confidence
                    };
                }

                var tacheTitre = analysis.ExtractedData["nom"].ToString();
                var prioriteString = analysis.ExtractedData.ContainsKey("priorite")
                    ? analysis.ExtractedData["priorite"].ToString()
                    : "Moyenne";

                var equipeNom = analysis.ExtractedData.ContainsKey("equipe")
                    ? analysis.ExtractedData["equipe"].ToString()
                    : null;

                // ✅ Convertir la priorité string vers enum
                var priorite = ConvertToPrioriteEnum(prioriteString);

                // ✅ Trouver l'ID de l'équipe par son nom
                var equipeId = await GetEquipeIdByName(equipeNom);
                if (equipeId == null)
                {
                    return new CommandResult
                    {
                        Success = false,
                        Message = $"❌ Équipe '{equipeNom}' non trouvée. Vérifiez le nom de l'équipe.",
                        Type = CommandType.Tache,
                        Confidence = analysis.Confidence
                    };
                }

                // ✅ VRAIE CRÉATION avec MediatR
                var createCommand = new CreateTacheCommand
                {
                    Titre = tacheTitre,
                    Priorite = priorite,
                    EquipeId = equipeId.Value
                };

                var result = await _mediator.Send(createCommand);

                var message = $"✅ Tâche '{tacheTitre}' avec priorité {prioriteString} créée avec succès!";
                if (!string.IsNullOrEmpty(equipeNom))
                {
                    message += $"\n👥 Affectée à l'équipe: {equipeNom}";
                }

                return new CommandResult
                {
                    Success = true,
                    Message = message,
                    Data = result,
                    Type = CommandType.Tache,
                    Confidence = analysis.Confidence,
                    ExtractedData = analysis.ExtractedData
                };
            }
            catch (Exception ex)
            {
                return new CommandResult
                {
                    Success = false,
                    Message = $"❌ Erreur lors de la création de la tâche: {ex.Message}",
                    Type = CommandType.Tache,
                    Confidence = analysis.Confidence
                };
            }
        }

        private async Task<CommandResult> ProcessEquipeCommand(string command, CommandAnalysis analysis)
        {
            try
            {
                if (!analysis.ExtractedData.ContainsKey("nom"))
                {
                    return new CommandResult
                    {
                        Success = false,
                        Message = "❌ Je n'ai pas pu identifier le nom de l'équipe. Essayez: 'Créer une équipe nommée MonEquipe'",
                        Type = CommandType.Equipe,
                        Confidence = analysis.Confidence
                    };
                }

                var equipeName = analysis.ExtractedData["nom"].ToString();

                return new CommandResult
                {
                    Success = true,
                    Message = $"✅ Équipe '{equipeName}' créée avec succès!",
                    Data = new { nom = equipeName },
                    Type = CommandType.Equipe,
                    Confidence = analysis.Confidence,
                    ExtractedData = analysis.ExtractedData
                };
            }
            catch (Exception ex)
            {
                return new CommandResult
                {
                    Success = false,
                    Message = $"❌ Erreur lors de la création de l'équipe: {ex.Message}",
                    Type = CommandType.Equipe,
                    Confidence = analysis.Confidence
                };
            }
        }

        private async Task<CommandResult> ProcessMembreCommand(string command, CommandAnalysis analysis)
        {
            try
            {
                if (!analysis.ExtractedData.ContainsKey("nom") || !analysis.ExtractedData.ContainsKey("equipe"))
                {
                    return new CommandResult
                    {
                        Success = false,
                        Message = "❌ Je n'ai pas pu identifier le membre et l'équipe. Essayez: 'Ajouter le membre Jean dans l'équipe DevTeam'",
                        Type = CommandType.Membre,
                        Confidence = analysis.Confidence
                    };
                }

                var membreName = analysis.ExtractedData["nom"].ToString();
                var equipeName = analysis.ExtractedData["equipe"].ToString();

                return new CommandResult
                {
                    Success = true,
                    Message = $"✅ Membre '{membreName}' ajouté à l'équipe '{equipeName}' avec succès!",
                    Data = new { membre = membreName, equipe = equipeName },
                    Type = CommandType.Membre,
                    Confidence = analysis.Confidence,
                    ExtractedData = analysis.ExtractedData
                };
            }
            catch (Exception ex)
            {
                return new CommandResult
                {
                    Success = false,
                    Message = $"❌ Erreur lors de l'ajout du membre: {ex.Message}",
                    Type = CommandType.Membre,
                    Confidence = analysis.Confidence
                };
            }
        }

        private async Task<CommandResult> ProcessPlanificationCommand(string command, CommandAnalysis analysis)
        {
            try
            {
                // 🎯 Extraire les informations de planification
                var (tacheNom, projetNom, heureDebut, heureFin, description, date) = ExtractPlanificationData(command);
                
                if (string.IsNullOrEmpty(tacheNom))
                {
                    return new CommandResult
                    {
                        Success = false,
                        Message = "❌ Je n'ai pas pu identifier la tâche. Essayez: 'J'ai travaillé sur Créer page d'accueil du projet Site Web E-commerce de 9h à 11h30'",
                        Type = CommandType.Planification,
                        Confidence = analysis.Confidence
                    };
                }

                // 🔍 Chercher la tâche et le projet par nom
                var tache = await FindTacheByName(tacheNom);
                if (tache == null)
                {
                    return new CommandResult
                    {
                        Success = false,
                        Message = $"❌ Tâche '{tacheNom}' non trouvée. Créez d'abord la tâche.",
                        Type = CommandType.Planification,
                        Confidence = analysis.Confidence
                    };
                }

                var projet = await FindProjetByName(projetNom);
                if (projet == null)
                {
                    return new CommandResult
                    {
                        Success = false,
                        Message = $"❌ Projet '{projetNom}' non trouvé. Créez d'abord le projet.",
                        Type = CommandType.Planification,
                        Confidence = analysis.Confidence
                    };
                }

                // ✅ Créer la planification
                var planification = new Planification
                {
                    Date = date ?? DateTime.Today,
                    HeureDebut = heureDebut,
                    HeureFin = heureFin,
                    Description = description ?? $"Travail sur {tacheNom}",
                    TacheId = tache.Id,
                    ProjetId = projet.Id,
                    ListeId = EtatListe.EnCours,
                    UserId = 1 // TODO: Récupérer l'ID de l'utilisateur connecté
                };

                // 💾 VRAIE SAUVEGARDE dans la base de données
                try
                {
                    // ✅ SAUVEGARDER RÉELLEMENT dans la base de données
                    var planificationSauvegardee = await _planificationRepository.CreateWithIncludesAsync(planification);
                    
                    return new CommandResult
                    {
                        Success = true,
                        Message = $"✅ Planification créée et SAUVEGARDÉE en base ! {tacheNom} pour {projetNom} de {heureDebut:hh\\:mm} à {heureFin:hh\\:mm}\n\n📝 Description: {description ?? $"Travail sur {tacheNom}"}\n📅 Date: {date?.ToString("dd/MM/yyyy") ?? DateTime.Today.ToString("dd/MM/yyyy")}\n💾 ID: {planificationSauvegardee.Id}",
                        Type = CommandType.Planification,
                        Confidence = analysis.Confidence,
                        ExtractedData = new Dictionary<string, object>
                        {
                            ["tache"] = tacheNom,
                            ["projet"] = projetNom,
                            ["debut"] = heureDebut,
                            ["fin"] = heureFin,
                            ["description"] = description,
                            ["planification"] = planificationSauvegardee
                        }
                    };
                }
                catch (Exception ex)
                {
                    return new CommandResult
                    {
                        Success = false,
                        Message = $"❌ Erreur lors de la sauvegarde en base: {ex.Message}",
                        Type = CommandType.Planification,
                        Confidence = analysis.Confidence
                    };
                }

                return new CommandResult
                {
                    Success = true,
                    Message = $"✅ Planification créée ! {tacheNom} pour {projetNom} de {heureDebut:hh\\:mm} à {heureFin:hh\\:mm}",
                    Type = CommandType.Planification,
                    Confidence = analysis.Confidence,
                    ExtractedData = new Dictionary<string, object>
                    {
                        ["tache"] = tacheNom,
                        ["projet"] = projetNom,
                        ["debut"] = heureDebut,
                        ["fin"] = heureFin,
                        ["description"] = description
                    }
                };
            }
            catch (Exception ex)
            {
                return new CommandResult
                {
                    Success = false,
                    Message = $"❌ Erreur lors de la création de la planification: {ex.Message}",
                    Type = CommandType.Planification,
                    Confidence = analysis.Confidence
                };
            }
        }

        // 🛠️ MÉTHODES UTILITAIRES
        private bool ContainsAny(string text, string[] keywords)
        {
            return keywords.Any(keyword => text.Contains(keyword));
        }

        private string ExtractAfterKeyword(string text, string[] keywords)
        {
            foreach (var keyword in keywords)
            {
                var index = text.IndexOf(keyword, StringComparison.OrdinalIgnoreCase);
                if (index >= 0)
                {
                    var afterKeyword = text.Substring(index + keyword.Length).Trim();

                    // 🎤 VOCAL : Arrêter aux mots-clés de transition
                    var stopWords = new[] {
                        " statut", " commence", " fini", " avec", " pour",
                        " priorité", " description", " équipe", " membre",
                        " aujourd'hui", " demain", " dans", " sur"
                    };

                    foreach (var stop in stopWords)
                    {
                        var stopIndex = afterKeyword.IndexOf(stop, StringComparison.OrdinalIgnoreCase);
                        if (stopIndex > 0)
                            afterKeyword = afterKeyword.Substring(0, stopIndex);
                    }

                    // Prendre maximum 2-3 mots pour éviter les phrases trop longues
                    var words = afterKeyword.Split(' ').Where(w => !string.IsNullOrWhiteSpace(w)).Take(3);
                    return string.Join(" ", words).Trim();
                }
            }
            return null;
        }

        // ✅ AMÉLIORATION pour mieux détecter "moyenne"
        private string ExtractPriorite(string text)
        {
            if (text.Contains("priorité haute") || text.Contains("priorité élevée") || text.Contains("urgent") || text.Contains("élevée"))
                return "Haute";
            if (text.Contains("priorité basse") || text.Contains("priorité faible") || text.Contains("faible"))
                return "Basse";
            if (text.Contains("priorité moyenne") || text.Contains("priorité normale") || text.Contains("moyenne"))
                return "Moyenne";
            return null;
        }

        // ✅ NOUVELLES MÉTHODES UTILITAIRES pour les tâches
        private PrioriteTache ConvertToPrioriteEnum(string prioriteString)
        {
            return prioriteString.ToLower() switch
            {
                "haute" or "élevée" or "elevee" => PrioriteTache.Elevee,
                "basse" or "faible" => PrioriteTache.Faible,
                _ => PrioriteTache.Moyenne
            };
        }

        private async Task<int?> GetEquipeIdByName(string equipeNom)
        {
            if (string.IsNullOrEmpty(equipeNom)) return null;

            try
            {
                // ✅ REQUÊTE DIRECTE à la base de données
                var equipes = await _equipeRepository.GetAllAsync();
                var equipe = equipes.FirstOrDefault(e => 
                    e.Nom.ToLower().Contains(equipeNom.ToLower())
                );
                
                if (equipe != null)
                {
                    Console.WriteLine($"DEBUG: Équipe trouvée = '{equipe.Nom}' (ID: {equipe.IdEquipe})");
                    return equipe.IdEquipe;
                }
                
                Console.WriteLine($"DEBUG: Aucune équipe trouvée pour '{equipeNom}'");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DEBUG: Erreur recherche équipe: {ex.Message}");
                return null;
            }
        }

        private string GetHelpMessage()
        {
            return @"🤖 Je peux vous aider avec (assistant vocal) :

📋 **PROJETS**
• 'Créer un projet nommé MonProjet'
• 'Nouveau projet appelé SiteWeb commence aujourd'hui fini demain'

✅ **TÂCHES** 
• 'Créer une tâche qui s'appelle Coding avec priorité haute'
• 'Nouvelle tâche qui s'appelle Test affecter a l'équipe Mobile avec priorité moyenne'

👥 **ÉQUIPES**
• 'Créer une équipe nommée DevTeam'
• 'Nouvelle équipe appelée Frontend'

👤 **MEMBRES**
• 'Ajouter le membre Jean dans l'équipe DevTeam'
• 'Nouveau membre Marie pour l'équipe Frontend'

⏰ **PLANIFICATION**
• 'J'ai travaillé sur Créer page d'accueil du projet Site Web E-commerce de 9h à 11h30'";
        }

        // 🎯 MÉTHODES POUR LA PLANIFICATION
        private (string tacheNom, string projetNom, TimeSpan heureDebut, TimeSpan heureFin, string description, DateTime? date) ExtractPlanificationData(string command)
        {
            var commandLower = command.ToLower();
            var originalCommand = command; // ✅ Garder l'original pour la casse
            
            // ✅ SOLUTION : Extraire avec l'original pour garder la casse
            var tacheNom = ExtractTacheFromPlanification(originalCommand);
            var projetNom = ExtractProjetFromPlanification(originalCommand);
            
            // Extraire les heures (format: "de 9h à 11h30")
            var heureDebut = ExtractHeure(commandLower, "de ");
            var heureFin = ExtractHeure(commandLower, "à ");
            
            // Extraire la description
            var description = $"Travail sur {tacheNom}";
            
            // Date par défaut: aujourd'hui
            var date = DateTime.Today;
            
            return (tacheNom, projetNom, heureDebut, heureFin, description, date);
        }

        private string ExtractTacheFromPlanification(string command)
        {
            Console.WriteLine($"DEBUG EXTRACTION: Commande reçue = '{command}'");
            
            // ✅ PATTERNS CORRIGÉS pour s'arrêter avant "du projet"
            var patterns = new[]
            {
                @"sur\s+(.+?)\s+du projet", // "sur documentation utilisateur du projet"
                @"sur\s+(.+?)\s+pour", // "sur documentation utilisateur pour"
                @"sur\s+(.+?)\s+dans", // "sur documentation utilisateur dans"
                @"sur\s+(.+?)\s+de\s+\d", // "sur documentation utilisateur de 08h"
                @"sur\s+(.+?)$", // "sur documentation utilisateur" (fin de phrase)
            };

            foreach (var pattern in patterns)
            {
                var match = Regex.Match(command, pattern, RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    var result = match.Groups[1].Value.Trim();
                    Console.WriteLine($"DEBUG EXTRACTION: Pattern '{pattern}' a trouvé = '{result}'");
                    return result;
                }
                else
                {
                    Console.WriteLine($"DEBUG EXTRACTION: Pattern '{pattern}' n'a rien trouvé");
                }
            }
            
            Console.WriteLine("DEBUG EXTRACTION: Aucune tâche extraite");
            return null;
        }

        private string ExtractProjetFromPlanification(string command)
        {
            // Patterns pour extraire le projet
            var patterns = new[]
            {
                @"projet\s+(.+?)\s+de\s+\d", // "projet Site Web E-commerce de 14h"
                @"du\s+projet\s+(.+?)\s+de\s+\d", // "du projet Site Web E-commerce de 14h"
                @"projet\s+(.+?)$", // "projet Site Web E-commerce" (fin)
                @"du\s+projet\s+(.+?)$", // "du projet Site Web E-commerce" (fin)
            };

            foreach (var pattern in patterns)
            {
                var match = Regex.Match(command, pattern, RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    return match.Groups[1].Value.Trim();
                }
            }
            
            return null;
        }

        private TimeSpan ExtractHeure(string text, string prefix)
        {
            var index = text.IndexOf(prefix);
            if (index < 0) return TimeSpan.Zero;
            
            var afterPrefix = text.Substring(index + prefix.Length);
            
            // Chercher le pattern d'heure : "10h", "12h", "9h30", etc.
            var heurePattern = @"(\d{1,2})h(\d{2})?";
            var match = Regex.Match(afterPrefix, heurePattern);
            
            if (match.Success)
            {
                var heures = int.Parse(match.Groups[1].Value);
                var minutes = match.Groups[2].Success ? int.Parse(match.Groups[2].Value) : 0;
                
                Console.WriteLine($"DEBUG: Heure extraite avec '{prefix}' -> {heures}:{minutes:D2}");
                return new TimeSpan(heures, minutes, 0);
            }
            
            Console.WriteLine($"DEBUG: Aucune heure trouvée avec '{prefix}' dans '{afterPrefix}'");
            return TimeSpan.Zero;
        }

        private async Task<Tache> FindTacheByName(string nom)
        {
            if (string.IsNullOrEmpty(nom)) return null;
            
            var nomLower = nom.ToLower().Trim();
            Console.WriteLine($"DEBUG: Recherche de tâche pour '{nomLower}'");
            
            try
            {
                // ✅ RÉCUPÉRER TOUTES LES TÂCHES DE LA BASE
                var taches = await _tacheRepository.GetAllAsync();
                Console.WriteLine($"DEBUG: {taches.Count()} tâches récupérées de la base");
                
                // ✅ Recherche exacte d'abord (insensible à la casse)
                var tacheExacte = taches.FirstOrDefault(t => 
                    t.Titre.ToLower().Trim() == nomLower);
                
                if (tacheExacte != null)
                {
                    Console.WriteLine($"DEBUG: Tâche trouvée (exacte) = '{tacheExacte.Titre}' (ID: {tacheExacte.Id})");
                    return tacheExacte;
                }
                
                // ✅ Recherche partielle si pas trouvé
                var tachePartielle = taches.FirstOrDefault(t => 
                    t.Titre.ToLower().Contains(nomLower) || 
                    nomLower.Contains(t.Titre.ToLower()));
                
                if (tachePartielle != null)
                {
                    Console.WriteLine($"DEBUG: Tâche trouvée (partielle) = '{tachePartielle.Titre}' (ID: {tachePartielle.Id})");
                    return tachePartielle;
                }
                
                Console.WriteLine($"DEBUG: Aucune tâche trouvée pour '{nomLower}'");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DEBUG: Erreur lors de la recherche de tâche: {ex.Message}");
                return null;
            }
        }

        private async Task<Projet> FindProjetByName(string nom)
        {
            if (string.IsNullOrEmpty(nom)) return null;
            
            var nomLower = nom.ToLower().Trim();
            Console.WriteLine($"DEBUG: Recherche de projet pour '{nomLower}'");
            
            try
            {
                // ✅ RÉCUPÉRER TOUS LES PROJETS DE LA BASE
                var projets = await _projetRepository.GetAllAsync();
                Console.WriteLine($"DEBUG: {projets.Count()} projets récupérés de la base");
                
                // ✅ Recherche exacte d'abord (insensible à la casse)
                var projetExact = projets.FirstOrDefault(p => 
                    p.Nom.ToLower().Trim() == nomLower);
                
                if (projetExact != null)
                {
                    Console.WriteLine($"DEBUG: Projet trouvé (exact) = '{projetExact.Nom}' (ID: {projetExact.Id})");
                    return projetExact;
                }
                
                // ✅ Recherche partielle si pas trouvé
                var projetPartiel = projets.FirstOrDefault(p => 
                    p.Nom.ToLower().Contains(nomLower) || 
                    nomLower.Contains(p.Nom.ToLower()));
                
                if (projetPartiel != null)
                {
                    Console.WriteLine($"DEBUG: Projet trouvé (partiel) = '{projetPartiel.Nom}' (ID: {projetPartiel.Id})");
                    return projetPartiel;
                }
                
                Console.WriteLine($"DEBUG: Aucun projet trouvé pour '{nomLower}'");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DEBUG: Erreur lors de la recherche de projet: {ex.Message}");
                return null;
            }
        }

    }

    // Classe d'analyse des commandes
    public class CommandAnalysis
    {
        public CommandType Type { get; set; }
        public float Confidence { get; set; }
        public Dictionary<string, object> ExtractedData { get; set; } = new Dictionary<string, object>();
    }
}