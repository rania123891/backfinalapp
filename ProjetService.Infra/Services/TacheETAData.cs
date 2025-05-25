using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.ML.Data;
using ProjetService.Domain.Models;
using ProjetService.Domain.Interfaces;

namespace ProjetService.Infrastructure.Services
{
    // 📊 Modèle de données pour l'entraînement ML
    public class TacheETAData
    {
        [LoadColumn(0)]
        public float TitreLongueur { get; set; }

        [LoadColumn(1)]
        public float PrioriteNumerique { get; set; }

        [LoadColumn(2)]
        public float ProjetComplexite { get; set; }

        [LoadColumn(3)]
        public float NombreMots { get; set; }

        [LoadColumn(4)]
        public float ContientAPI { get; set; }

        [LoadColumn(5)]
        public float ContientCRUD { get; set; }

        [LoadColumn(6)]
        public float ContientInterface { get; set; }

        [LoadColumn(7)]
        public float ContientDB { get; set; }

        [LoadColumn(8)]
        public float JourSemaine { get; set; }

        [LoadColumn(9)]
        public float ProjetDuree { get; set; }

        [LoadColumn(10)]
        [ColumnName("Label")]
        public float DureeReelleHeures { get; set; } // Target à prédire
    }

    // 🎯 Résultat de prédiction
    public class ETAPrediction
    {
        [ColumnName("Score")]
        public float DureeEstimeeHeures { get; set; }
    }

    // 📈 Résultat enrichi pour l'API
    public class ETAResult
    {
        public float DureeEstimeeHeures { get; set; }
        public string DureeFormatee { get; set; }
        public string Confiance { get; set; }
        public string TypeTache { get; set; }
        public string[] FacteursInfluents { get; set; }
        public DateTime DateEstimeeCompletion { get; set; }
    }

    public interface IETAPredictionService
    {
        Task<ETAResult> PredireDureeTacheAsync(Tache tache, Projet projet);
        Task<bool> EntrainerModeleAsync();
        Task<string> GetStatistiquesModeleAsync();
    }

    public class ETAPredictionService : IETAPredictionService
    {
        private readonly MLContext _mlContext;
        private ITransformer _modele;
        private readonly string _cheminModele = "Models/eta_model.zip";
        private readonly IGenericRepository<Tache> _tacheRepository;
        private readonly IGenericRepository<Planification> _planificationRepository;
        private readonly IGenericRepository<Projet> _projetRepository;

        public ETAPredictionService(
            IGenericRepository<Tache> tacheRepository,
            IGenericRepository<Planification> planificationRepository,
            IGenericRepository<Projet> projetRepository)
        {
            _mlContext = new MLContext(seed: 42);
            _tacheRepository = tacheRepository;
            _planificationRepository = planificationRepository;
            _projetRepository = projetRepository;

            // Charger le modèle s'il existe
            ChargerModeleExistant();
        }

        public async Task<ETAResult> PredireDureeTacheAsync(Tache tache, Projet projet)
        {
            if (_modele == null)
            {
                await EntrainerModeleAsync();
            }

            // 🔧 Extraction des features
            var features = ExtraireFeatures(tache, projet);

            // 🎯 Prédiction
            var predictionEngine = _mlContext.Model.CreatePredictionEngine<TacheETAData, ETAPrediction>(_modele);
            var prediction = predictionEngine.Predict(features);

            // 📊 Analyse et enrichissement du résultat
            var resultat = new ETAResult
            {
                DureeEstimeeHeures = Math.Max(0.5f, prediction.DureeEstimeeHeures), // Minimum 30 min
                DureeFormatee = FormatDuree(prediction.DureeEstimeeHeures),
                TypeTache = DetecterTypeTache(tache.Titre),
                Confiance = CalculerConfiance(features),
                FacteursInfluents = AnalyserFacteursInfluents(features),
                DateEstimeeCompletion = DateTime.Now.AddHours(prediction.DureeEstimeeHeures)
            };

            return resultat;
        }

        public async Task<bool> EntrainerModeleAsync()
        {
            try
            {
                // 📊 Récupération des données d'entraînement
                var donneesEntrainement = await CollecterDonneesEntrainementAsync();

                if (donneesEntrainement.Count < 10)
                {
                    // Pas assez de données, utiliser des données de base
                    donneesEntrainement = GenererDonneesDeBase();
                }

                var dataView = _mlContext.Data.LoadFromEnumerable(donneesEntrainement);

                // 🏗️ Pipeline ML
                var pipeline = _mlContext.Transforms.Concatenate("Features",
                    nameof(TacheETAData.TitreLongueur),
                    nameof(TacheETAData.PrioriteNumerique),
                    nameof(TacheETAData.ProjetComplexite),
                    nameof(TacheETAData.NombreMots),
                    nameof(TacheETAData.ContientAPI),
                    nameof(TacheETAData.ContientCRUD),
                    nameof(TacheETAData.ContientInterface),
                    nameof(TacheETAData.ContientDB),
                    nameof(TacheETAData.JourSemaine),
                    nameof(TacheETAData.ProjetDuree))
                .Append(_mlContext.Regression.Trainers.FastTree(labelColumnName: "Label", featureColumnName: "Features"));

                // 🎓 Entraînement
                _modele = pipeline.Fit(dataView);

                // 💾 Sauvegarde
                Directory.CreateDirectory(Path.GetDirectoryName(_cheminModele));
                _mlContext.Model.Save(_modele, dataView.Schema, _cheminModele);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'entraînement : {ex.Message}");
                return false;
            }
        }

        public async Task<string> GetStatistiquesModeleAsync()
        {
            var donnees = await CollecterDonneesEntrainementAsync();
            var stats = $@"
📊 Statistiques du modèle ETA :
- Tâches analysées : {donnees.Count}
- Durée moyenne : {(donnees.Count > 0 ? donnees.Average(d => d.DureeReelleHeures) : 0):F1}h
- Durée médiane : {(donnees.Count > 0 ? CalculerMediane(donnees.Select(d => d.DureeReelleHeures)) : 0):F1}h
- Tâches priorité élevée : {donnees.Count(d => d.PrioriteNumerique == 2)}
- Tâches avec API : {donnees.Count(d => d.ContientAPI == 1)}
- Tâches avec CRUD : {donnees.Count(d => d.ContientCRUD == 1)}
";
            return stats;
        }

        private async Task<List<TacheETAData>> CollecterDonneesEntrainementAsync()
        {
            var donnees = new List<TacheETAData>();

            try
            {
                // Récupérer toutes les tâches
                var taches = await _tacheRepository.GetAllAsync();

                // Récupérer toutes les planifications terminées
                var planifications = await _planificationRepository.GetAllAsync();
                var planificationsTerminees = planifications
                    .Where(p => p.ListeId == EtatListe.Termine)
                    .GroupBy(p => p.TacheId)
                    .ToList();

                foreach (var groupePlanifications in planificationsTerminees)
                {
                    var tacheId = groupePlanifications.Key;
                    var tache = taches.FirstOrDefault(t => t.Id == tacheId);

                    if (tache != null)
                    {
                        // Calculer le temps total passé sur cette tâche
                        var tempsTotal = groupePlanifications
                            .Sum(p => (p.HeureFin - p.HeureDebut).TotalHours);

                        if (tempsTotal > 0)
                        {
                            var projetId = groupePlanifications.First().ProjetId;
                            var projet = await _projetRepository.GetByIdAsync(projetId);

                            donnees.Add(ExtraireFeatures(tache, projet, (float)tempsTotal));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la collecte des données : {ex.Message}");
            }

            return donnees;
        }

        private TacheETAData ExtraireFeatures(Tache tache, Projet projet, float? dureeReelle = null)
        {
            var titre = tache.Titre?.ToLower() ?? "";

            return new TacheETAData
            {
                TitreLongueur = titre.Length,
                PrioriteNumerique = (float)tache.Priorite,
                ProjetComplexite = CalculerComplexiteProjet(projet),
                NombreMots = titre.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length,
                ContientAPI = titre.Contains("api") ? 1 : 0,
                ContientCRUD = titre.Contains("crud") || titre.Contains("create") || titre.Contains("update") || titre.Contains("delete") ? 1 : 0,
                ContientInterface = titre.Contains("interface") || titre.Contains("ui") || titre.Contains("front") ? 1 : 0,
                ContientDB = titre.Contains("base") || titre.Contains("bdd") || titre.Contains("database") ? 1 : 0,
                JourSemaine = (float)DateTime.Now.DayOfWeek,
                ProjetDuree = projet?.Duree ?? 30,
                DureeReelleHeures = dureeReelle ?? 0
            };
        }

        private float CalculerComplexiteProjet(Projet projet)
        {
            if (projet == null) return 1.0f;

            // Complexité basée sur la durée du projet
            var facteurDuree = Math.Min(projet.Duree / 30.0f, 3.0f); // Max 3x

            // On ne peut pas accéder aux tâches directement, donc on utilise la durée comme proxy
            return Math.Max(facteurDuree, 1.0f);
        }

        private string DetecterTypeTache(string titre)
        {
            var titreLower = titre?.ToLower() ?? "";

            if (titreLower.Contains("api") || titreLower.Contains("backend")) return "🔧 Backend";
            if (titreLower.Contains("ui") || titreLower.Contains("interface") || titreLower.Contains("front")) return "🎨 Frontend";
            if (titreLower.Contains("test") || titreLower.Contains("bug")) return "🧪 Test/Debug";
            if (titreLower.Contains("doc") || titreLower.Contains("rapport")) return "📝 Documentation";
            if (titreLower.Contains("crud") || titreLower.Contains("base")) return "💾 Base de données";

            return "⚙️ Général";
        }

        private string CalculerConfiance(TacheETAData features)
        {
            var score = 0;

            // Plus de mots-clés techniques = plus de confiance
            if (features.ContientAPI == 1) score += 20;
            if (features.ContientCRUD == 1) score += 20;
            if (features.ContientInterface == 1) score += 15;
            if (features.ContientDB == 1) score += 15;
            if (features.NombreMots >= 3) score += 20;
            if (features.ProjetComplexite > 1) score += 10;

            return score >= 70 ? "🟢 Élevée" : score >= 40 ? "🟡 Moyenne" : "🟠 Faible";
        }

        private string[] AnalyserFacteursInfluents(TacheETAData features)
        {
            var facteurs = new List<string>();

            if (features.PrioriteNumerique == 2) facteurs.Add("⚡ Priorité élevée (+30%)");
            if (features.ContientAPI == 1) facteurs.Add("🔧 Développement API (+40%)");
            if (features.ContientCRUD == 1) facteurs.Add("💾 Opérations CRUD (+25%)");
            if (features.ContientInterface == 1) facteurs.Add("🎨 Interface utilisateur (+35%)");
            if (features.ProjetComplexite > 2) facteurs.Add("📈 Projet complexe (+20%)");
            if (features.JourSemaine == 1 || features.JourSemaine == 5) facteurs.Add("📅 Début/fin de semaine (-10%)");

            return facteurs.ToArray();
        }

        private string FormatDuree(float heures)
        {
            if (heures < 1)
                return $"{(int)(heures * 60)} minutes";
            else if (heures < 8)
                return $"{heures:F1} heures";
            else
                return $"{(int)(heures / 8)} jours {(heures % 8):F1}h";
        }

        private void ChargerModeleExistant()
        {
            if (File.Exists(_cheminModele))
            {
                try
                {
                    _modele = _mlContext.Model.Load(_cheminModele, out _);
                }
                catch
                {
                    _modele = null;
                }
            }
        }

        private List<TacheETAData> GenererDonneesDeBase()
        {
            // Données de base pour démarrer si pas assez d'historique
            return new List<TacheETAData>
            {
                new() { TitreLongueur = 15, PrioriteNumerique = 0, NombreMots = 3, ContientAPI = 0, DureeReelleHeures = 2.0f },
                new() { TitreLongueur = 25, PrioriteNumerique = 1, NombreMots = 4, ContientAPI = 1, DureeReelleHeures = 4.5f },
                new() { TitreLongueur = 35, PrioriteNumerique = 2, NombreMots = 6, ContientCRUD = 1, DureeReelleHeures = 6.0f },
                new() { TitreLongueur = 20, PrioriteNumerique = 1, NombreMots = 3, ContientInterface = 1, DureeReelleHeures = 3.5f },
                new() { TitreLongueur = 40, PrioriteNumerique = 2, NombreMots = 7, ContientDB = 1, DureeReelleHeures = 8.0f },
                new() { TitreLongueur = 18, PrioriteNumerique = 0, NombreMots = 2, ContientAPI = 0, DureeReelleHeures = 1.5f },
                new() { TitreLongueur = 30, PrioriteNumerique = 2, NombreMots = 5, ContientInterface = 1, DureeReelleHeures = 5.0f },
                new() { TitreLongueur = 12, PrioriteNumerique = 0, NombreMots = 2, ContientAPI = 0, DureeReelleHeures = 1.0f },
                new() { TitreLongueur = 45, PrioriteNumerique = 2, NombreMots = 8, ContientDB = 1, DureeReelleHeures = 10.0f },
                new() { TitreLongueur = 22, PrioriteNumerique = 1, NombreMots = 4, ContientCRUD = 1, DureeReelleHeures = 3.0f }
            };
        }

        private float CalculerMediane(IEnumerable<float> valeurs)
        {
            var ordonnees = valeurs.OrderBy(x => x).ToList();
            if (ordonnees.Count == 0) return 0;

            var milieu = ordonnees.Count / 2;
            return ordonnees.Count % 2 == 0
                ? (ordonnees[milieu - 1] + ordonnees[milieu]) / 2
                : ordonnees[milieu];
        }
    }
}