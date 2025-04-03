using NJsonSchema;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using WebSocketBoilerplate;

namespace Startup.Documentation;

/// <summary>
///     I want nswag to include event type names like "ClientWantsToDoX" as simple string constants
/// </summary>
public sealed class AddStringConstantsProcessor : IDocumentProcessor
{
    public void Process(DocumentProcessorContext context)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        var derivedTypeNames = assemblies
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
                typeof(BaseDto).IsAssignableFrom(t)
            )
            .Select(t => t.Name)
            .ToArray();

        var schema = new JsonSchema
        {
            Type = JsonObjectType.String,
            Description = "Available eventType constants"
        };

        foreach (var typeName in derivedTypeNames) schema.Enumeration.Add(typeName);

        context.Document.Definitions["StringConstants"] = schema;
    }
}