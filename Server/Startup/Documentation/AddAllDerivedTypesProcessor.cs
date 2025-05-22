using Application.Models.Dtos;
using Namotion.Reflection;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using WebSocketBoilerplate;

namespace Startup.Documentation;

/// <summary>
///     I want nswag to generate schemas for all derived types of BaseDto
/// </summary>
public sealed class AddAllDerivedTypesProcessor : IDocumentProcessor
{
    public void Process(DocumentProcessorContext context)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        var derivedTypes = assemblies
            .SelectMany(a =>
            {
                try
                {
                    return a.GetTypes();
                }
                catch
                {
                    return Array.Empty<Type>();
                }
            })
            //Here I'm actively looking for types used by the Websocket API
            //(ApplicationBaseDto is also relevant because Mqtt Infrastructure doesn't have websocket dependencies,
            // but I still want the inheritors to be documented by openapi)
            .Where(t =>
                t != typeof(BaseDto) &&
                !t.IsAbstract &&
                typeof(BaseDto).IsAssignableFrom(t) ||
                typeof(ApplicationBaseDto).IsAssignableFrom(t)
            )
            .ToList();

        foreach (var type in derivedTypes)
            context.SchemaGenerator.Generate(type.ToContextualType(), context.SchemaResolver);
    }
}