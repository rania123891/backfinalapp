using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace UserService.Api.Filters; // Adaptez le namespace selon votre structure

public class FormFileSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type == typeof(IFormFile) ||
            context.Type == typeof(IEnumerable<IFormFile>) ||
            context.Type == typeof(List<IFormFile>))
        {
            schema.Type = "string";
            schema.Format = "binary";
            schema.Description = "Fichier à uploader";

            // Si vous voulez aussi gérer les tableaux de fichiers
            if (context.Type != typeof(IFormFile))
            {
                schema.Type = "array";
                schema.Items = new OpenApiSchema
                {
                    Type = "string",
                    Format = "binary"
                };
            }
        }
    }
}