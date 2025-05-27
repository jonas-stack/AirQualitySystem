using System.Reflection;
using NJsonSchema;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using WebSocketBoilerplate;

namespace Startup.Documentation;

public sealed class AddWebsocketEventsProcessor : IDocumentProcessor
{
    public void Process(DocumentProcessorContext context)
    {
        var topicsType = typeof(Application.Models.Websocket.WebsocketEvents);

        var constants = topicsType
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(f => f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(string))
            .Select(f => f.GetRawConstantValue() as string)
            .Where(value => value != null)
            .Distinct()
            .ToList();

        var baseDtoType = typeof(BaseDto);
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        var derivedTypeNames = assemblies
            .SelectMany(a =>
            {
                try { return a.GetTypes(); }
                catch { return Array.Empty<Type>(); }
            })
            .Where(t =>
                t != baseDtoType &&
                !t.IsAbstract &&
                baseDtoType.IsAssignableFrom(t))
            .Select(t => t.Name)
            .Distinct()
            .ToList();

        var combinedEnumValues = constants
            .Concat(derivedTypeNames)
            .Distinct()
            .OrderBy(x => x)
            .ToList();

        var schema = new JsonSchema
        {
            Type = JsonObjectType.String,
            Description = "Websocket event types (constants + BaseDto inheritors)"
        };

        foreach (var val in combinedEnumValues)
            schema.Enumeration.Add(val);
        
        context.Document.Definitions["WebsocketEvents"] = schema;
    }
}
