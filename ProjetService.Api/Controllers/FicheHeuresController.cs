using Microsoft.AspNetCore.Mvc;
using MediatR;
using ProjetService.Domain.Queries;
using ProjetService.Domain.DTO;
using System.Text;

namespace ProjetService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FicheHeuresController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<FicheHeuresController> _logger;

        public FicheHeuresController(IMediator mediator, ILogger<FicheHeuresController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("rapport")]
        public async Task<ActionResult<FicheHeureResult>> GetRapportHeures([FromQuery] GetFicheHeureQuery query)
        {
            try
            {
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la génération du rapport d'heures");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        [HttpGet("pdf")]
        public async Task<IActionResult> GeneratePDF([FromQuery] GetFicheHeureQuery query)
        {
            try
            {
                var result = await _mediator.Send(query);
                
                if (result == null)
                    return NotFound("Aucune données trouvées pour générer le rapport");

                // Générer un PDF simple (pour l'instant, on retourne du HTML)
                var htmlContent = GenerateHtmlReport(result);
                var bytes = Encoding.UTF8.GetBytes(htmlContent);
                
                return File(bytes, "text/html", $"rapport_heures_{result.ProjetId}_{DateTime.Now:yyyyMMdd}.html");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la génération du PDF");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        [HttpGet("csv")]
        public async Task<IActionResult> GenerateCSV([FromQuery] GetFicheHeureQuery query)
        {
            try
            {
                var result = await _mediator.Send(query);
                
                if (result == null)
                    return NotFound("Aucune données trouvées pour générer le rapport");

                var csvContent = GenerateCsvReport(result);
                var bytes = Encoding.UTF8.GetBytes(csvContent);
                
                return File(bytes, "text/csv", $"rapport_heures_{result.ProjetId}_{DateTime.Now:yyyyMMdd}.csv");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la génération du CSV");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        [HttpGet("excel")]
        public async Task<IActionResult> GenerateExcel([FromQuery] GetFicheHeureQuery query)
        {
            try
            {
                var result = await _mediator.Send(query);
                
                if (result == null)
                    return NotFound("Aucune données trouvées pour générer le rapport");

                // Pour l'instant, retourne du CSV avec extension Excel
                // TODO: Implémenter la génération Excel réelle avec EPPlus
                var csvContent = GenerateCsvReport(result);
                var bytes = Encoding.UTF8.GetBytes(csvContent);
                
                return File(bytes, "application/vnd.ms-excel", $"rapport_heures_{result.ProjetId}_{DateTime.Now:yyyyMMdd}.csv");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la génération d'Excel");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        private string GenerateHtmlReport(FicheHeureResult result)
        {
            var html = new StringBuilder();
            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html lang='fr'>");
            html.AppendLine("<head>");
            html.AppendLine("<meta charset='UTF-8'>");
            html.AppendLine("<title>Rapport d'heures</title>");
            html.AppendLine("<style>");
            html.AppendLine("body { font-family: Arial, sans-serif; margin: 20px; }");
            html.AppendLine("h1 { color: #333; }");
            html.AppendLine("table { border-collapse: collapse; width: 100%; margin-top: 20px; }");
            html.AppendLine("th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }");
            html.AppendLine("th { background-color: #f2f2f2; }");
            html.AppendLine(".summary { background-color: #f9f9f9; padding: 15px; margin-bottom: 20px; }");
            html.AppendLine("</style>");
            html.AppendLine("</head>");
            html.AppendLine("<body>");

            html.AppendLine($"<h1>Rapport d'heures - {result.NomProjet}</h1>");
            html.AppendLine($"<p>Période : {result.DateDebut:dd/MM/yyyy} - {result.DateFin:dd/MM/yyyy}</p>");

            html.AppendLine("<div class='summary'>");
            html.AppendLine($"<h2>Résumé</h2>");
            html.AppendLine($"<p><strong>Total d'heures :</strong> {result.TotalHeures:F2} heures</p>");
            html.AppendLine($"<p><strong>Nombre de saisies :</strong> {result.SaisiesTemps.Count}</p>");
            html.AppendLine("</div>");

            if (result.SaisiesTemps.Any())
            {
                html.AppendLine("<h2>Détail des saisies</h2>");
                html.AppendLine("<table>");
                html.AppendLine("<tr><th>Date</th><th>Utilisateur</th><th>Tâche</th><th>Début</th><th>Fin</th><th>Durée</th><th>Description</th></tr>");

                foreach (var saisie in result.SaisiesTemps.OrderBy(s => s.DateTravail))
                {
                    html.AppendLine("<tr>");
                    html.AppendLine($"<td>{saisie.DateTravail:dd/MM/yyyy}</td>");
                    html.AppendLine($"<td>{saisie.UtilisateurId}</td>");
                    html.AppendLine($"<td>{saisie.TacheId?.ToString() ?? "N/A"}</td>");
                    html.AppendLine($"<td>{saisie.HeureDebut}</td>");
                    html.AppendLine($"<td>{saisie.HeureFin}</td>");
                    html.AppendLine($"<td>{saisie.DureeHeures:F2}h</td>");
                    html.AppendLine($"<td>{saisie.Description ?? ""}</td>");
                    html.AppendLine("</tr>");
                }

                html.AppendLine("</table>");
            }

            if (result.HeuresParUtilisateur.Any())
            {
                html.AppendLine("<h2>Heures par utilisateur</h2>");
                html.AppendLine("<table>");
                html.AppendLine("<tr><th>Utilisateur</th><th>Total heures</th></tr>");

                foreach (var kvp in result.HeuresParUtilisateur)
                {
                    html.AppendLine("<tr>");
                    html.AppendLine($"<td>{kvp.Key}</td>");
                    html.AppendLine($"<td>{kvp.Value:F2}h</td>");
                    html.AppendLine("</tr>");
                }

                html.AppendLine("</table>");
            }

            html.AppendLine("</body>");
            html.AppendLine("</html>");

            return html.ToString();
        }

        private string GenerateCsvReport(FicheHeureResult result)
        {
            var csv = new StringBuilder();
            csv.AppendLine("Date,Utilisateur,Tache,Heure Debut,Heure Fin,Duree,Description");

            foreach (var saisie in result.SaisiesTemps.OrderBy(s => s.DateTravail))
            {
                csv.AppendLine($"{saisie.DateTravail:dd/MM/yyyy},{saisie.UtilisateurId},{saisie.TacheId?.ToString() ?? "N/A"},{saisie.HeureDebut},{saisie.HeureFin},{saisie.DureeHeures:F2},\"{saisie.Description ?? ""}\"");
            }

            return csv.ToString();
        }
    }
}
