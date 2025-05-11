using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NJsonSchema.CodeGeneration.TypeScript;
using NSwag.CodeGeneration.TypeScript;
using NSwag.Generation;

namespace Startup.Documentation;

public static class GenerateTypescriptClient
{
    public static async Task GenerateTypeScriptClient(this WebApplication app, string path)
    {
        var document = await app.Services.GetRequiredService<IOpenApiDocumentGenerator>()
            .GenerateAsync("v1");
        var settings = new TypeScriptClientGeneratorSettings
        {
            Template = TypeScriptTemplate.Fetch,
            TypeScriptGeneratorSettings =
            {
                TypeStyle = TypeScriptTypeStyle.Interface,
                DateTimeType = TypeScriptDateTimeType.Date,
                NullValue = TypeScriptNullValue.Undefined,
                TypeScriptVersion = 5.2m,
                GenerateCloneMethod = false,
                MarkOptionalProperties = true
            }
        };


        var generator = new TypeScriptClientGenerator(document, settings);
        var code = generator.GenerateFile();

        // alex metode inkluderede stadigvæk baseDto.. ændret til et regex
        string pattern = @"export\s+interface\s+BaseDto\s*\{\s*eventType\?\:\s*string\;\s*requestId\?\:\s*string\;\s*\}";
        var modifiedCode = Regex.Replace(code, pattern, string.Empty);

        modifiedCode = "import { BaseDto } from 'ws-request-hook';" + Environment.NewLine + modifiedCode;
    
        var outputPath = Path.Combine(Directory.GetCurrentDirectory() + path);
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);

        await File.WriteAllTextAsync(outputPath, modifiedCode);
        app.Services.GetRequiredService<ILogger<Program>>()
            .LogInformation("TypeScript client generated at: " + outputPath);
    }
}