using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ProjetService.Domain.Models;
using ProjetService.Domain.Interfaces;
using ProjetService.Domain.Interface;

namespace ProjetService.Infra.Services
{
    public class CommandProcessor : ICommandProcessor
    {
        private readonly IGenericRepository<Projet> _projetRepository;

        public CommandProcessor(IGenericRepository<Projet> projetRepository)
        {
            _projetRepository = projetRepository;
        }

        public async Task<string> ProcessAsync(string command)
        {
            if (command.ToLower().Contains("projet"))
            {
                var infos = ExtraireInfos(command);
                if (string.IsNullOrWhiteSpace(infos.nom) || infos.echeance == null)
                    return "❌ Commande incomplète : nom ou date d’échéance manquant.";

                var dateDebut = DateTime.Now;
                var duree = (infos.echeance.Value - dateDebut).Days;

                var projet = new Projet
                {
                    Nom = infos.nom,
                    Description = infos.objectif ?? "Objectif par défaut",
                    DateDebut = dateDebut,
                    DateEcheance = infos.echeance.Value,
                    Duree = duree,
                    Statut = StatutProjet.EnCours,
                    CreateurId = 1
                };

                await _projetRepository.AddAsync(projet);

                return $"✅ Projet '{projet.Nom}' créé avec succès !\n📌 Objectif : {projet.Description}\n📅 Échéance : {projet.DateEcheance:dd/MM/yyyy}\n⏳ Durée : {projet.Duree} jours";
            }

            return "Commande non reconnue.";
        }

        private (string nom, string objectif, DateTime? echeance) ExtraireInfos(string commande)
        {
            string nom = null;
            string objectif = null;
            DateTime? dateEcheance = null;

            var nomMatch = Regex.Match(commande, @"(?:(nommé|appelé|intitulé)\s+'([^']+))", RegexOptions.IgnoreCase);
            if (nomMatch.Success)
                nom = nomMatch.Groups[2].Value;

            var objMatch = Regex.Match(commande, @"(?:vise à|pour)\s+([^\.]+)", RegexOptions.IgnoreCase);
            if (objMatch.Success)
                objectif = objMatch.Groups[1].Value;

            var dateMatch = Regex.Match(commande, @"le\s+(\d{1,2})\s+([a-zéû]+)\s+(\d{4})", RegexOptions.IgnoreCase);
            if (dateMatch.Success)
            {
                try
                {
                    int jour = int.Parse(dateMatch.Groups[1].Value);
                    string moisStr = dateMatch.Groups[2].Value.ToLower();
                    int annee = int.Parse(dateMatch.Groups[3].Value);

                    var moisFr = new Dictionary<string, int>
                    {
                        ["janvier"] = 1,
                        ["février"] = 2,
                        ["mars"] = 3,
                        ["avril"] = 4,
                        ["mai"] = 5,
                        ["juin"] = 6,
                        ["juillet"] = 7,
                        ["août"] = 8,
                        ["septembre"] = 9,
                        ["octobre"] = 10,
                        ["novembre"] = 11,
                        ["décembre"] = 12
                    };

                    if (moisFr.ContainsKey(moisStr))
                        dateEcheance = new DateTime(annee, moisFr[moisStr], jour);
                }
                catch { }
            }

            return (nom, objectif, dateEcheance);
        }
    }
}
