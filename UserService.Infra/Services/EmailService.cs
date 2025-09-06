using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using UserService.Domain.Interfaces.Infrastructure;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace UserService.Infra.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration config, ILogger<EmailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendVerificationEmail(string email, string verificationLink)
        {
            var apiKey = _config["EmailSettings:SendGridApiKey"];
            var client = new SendGridClient(apiKey);

            var from = new EmailAddress(
                _config["EmailSettings:FromEmail"],
                _config["EmailSettings:FromName"]);

            var to = new EmailAddress(email);
            var subject = "🔐 Vérifiez votre adresse email - PFE Application";
            
            var plainTextContent = $@"
Bonjour,

Merci de vous être inscrit sur notre plateforme !

Pour activer votre compte, veuillez cliquer sur le lien suivant :
{verificationLink}

Ce lien expirera dans 24 heures.

Si vous n'avez pas créé de compte, ignorez cet email.

Cordialement,
L'équipe PFE Application
            ";

            var htmlContent = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: #4CAF50; color: white; padding: 20px; text-align: center; }}
        .content {{ padding: 30px; background: #f9f9f9; }}
        .button {{ 
            display: inline-block; 
            background: #4CAF50; 
            color: white; 
            padding: 15px 30px; 
            text-decoration: none; 
            border-radius: 5px; 
            margin: 20px 0; 
        }}
        .footer {{ padding: 20px; text-align: center; font-size: 12px; color: #666; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🔐 Vérification d'email</h1>
        </div>
        <div class='content'>
            <h2>Bonjour !</h2>
            <p>Merci de vous être inscrit sur notre <strong>plateforme de gestion de projet</strong> !</p>
            
            <p>Pour activer votre compte et commencer à utiliser nos services, veuillez cliquer sur le bouton ci-dessous :</p>
            
            <div style='text-align: center;'>
                <a href='{verificationLink}' class='button'>✅ Vérifier mon email</a>
            </div>
            
            <p><small>Ou copiez ce lien dans votre navigateur :<br>
            <a href='{verificationLink}'>{verificationLink}</a></small></p>
            
            <p><strong>⏰ Important :</strong> Ce lien expirera dans <strong>24 heures</strong>.</p>
            
            <p>Si vous n'avez pas créé de compte, vous pouvez ignorer cet email en toute sécurité.</p>
        </div>
        <div class='footer'>
            <p>© 2024 PFE Application - Plateforme de Gestion de Projet</p>
            <p>Cet email a été envoyé automatiquement, merci de ne pas y répondre.</p>
        </div>
    </div>
</body>
</html>
            ";

            var msg = MailHelper.CreateSingleEmail(
                from, to, subject, plainTextContent, htmlContent);

            var response = await client.SendEmailAsync(msg);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Échec envoi email à {email}. Status: {response.StatusCode}");
                throw new Exception("Échec de l'envoi de l'email");
            }
        }
    }
}