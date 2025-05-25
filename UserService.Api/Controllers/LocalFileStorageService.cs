using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;
using UserService.Domain.Interfaces.Infrastructure;

namespace UserService.Api.Controllers
{
    public class LocalFileStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _environment;

        public LocalFileStorageService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> SaveFileAsync(IFormFile file, string folder)
        {
            // 🔧 Correction : gérer le cas où WebRootPath est null
            var baseUploadPath = _environment.WebRootPath ??
                                Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            Console.WriteLine($"💾 Base upload path: {baseUploadPath}");

            var uploadPath = Path.Combine(baseUploadPath, folder);
            Console.WriteLine($"📁 Upload folder: {uploadPath}");

            // Créer les dossiers s'ils n'existent pas
            Directory.CreateDirectory(uploadPath);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadPath, fileName);

            Console.WriteLine($"💾 Saving file to: {filePath}");

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            Console.WriteLine($"✅ File saved successfully: {fileName}");

            // Vérifier que le fichier existe bien
            if (File.Exists(filePath))
            {
                Console.WriteLine($"✅ File exists on disk: {filePath}");
            }
            else
            {
                Console.WriteLine($"❌ File NOT found on disk: {filePath}");
            }

            return $"/{folder}/{fileName}";
        }
    }
}