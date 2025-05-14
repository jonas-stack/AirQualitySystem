using System.Reflection;
using NJsonSchema;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace Startup.Documentation;

public sealed class AddWebsocketEventsProcessor : IDocumentProcessor
{
    public void Process(DocumentProcessorContext context)
    {
        var topicsType = typeof(Application.Models.WebsocketEvents);

        var constants = topicsType
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(f => f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(string))
            .Select(f => (string)f.GetRawConstantValue())
            .Distinct()
            .ToList();

        var schema = new JsonSchema
        {
            Type = JsonObjectType.String,
            Description = "Websocket Events enums"
        };

        foreach (var topic in constants)
            schema.Enumeration.Add(topic);

        context.Document.Definitions["WebsocketEvents"] = schema;
    }
}