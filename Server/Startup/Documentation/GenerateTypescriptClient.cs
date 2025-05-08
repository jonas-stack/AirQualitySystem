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

        var lines = code.Split(new[] { Environment.NewLine }, StringSplitOptions.None).ToList();
        var startIndex = lines.FindIndex(l => l.Contains("export interface BaseDto"));
        if (startIndex >= 0 && lines.Count >= startIndex + 4)
            lines.RemoveRange(startIndex, 4); // Remove 3 lines (interface declaration and two properties)

        lines.Insert(0, "import { BaseDto } from 'ws-request-hook';");

        var modifiedCode = string.Join(Environment.NewLine, lines);

        var outputPath = Path.Combine(Directory.GetCurrentDirectory() + path);
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);

        await File.WriteAllTextAsync(outputPath, modifiedCode);
        app.Services.GetRequiredService<ILogger<Program>>()
            .LogInformation("TypeScript client generated at: " + outputPath);
    }
}