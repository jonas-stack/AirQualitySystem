using System.Reflection;
using NJsonSchema;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace Startup.Documentation;

public sealed class AddWebsocketTopicsEnumProcessor : IDocumentProcessor
{
    public void Process(DocumentProcessorContext context)
    {
        var topicsType = typeof(Application.Models.Websocket.WebsocketTopics);

        var constants = topicsType
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(f => f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(string))
            .Select(f => f.GetRawConstantValue() as string)
            .Where(value => value != null)
            .Distinct()
            .ToList();
        var schema = new JsonSchema
        {
            Type = JsonObjectType.String,
            Description = "Websocket Topic Enums"
        };

        foreach (var topic in constants)
            schema.Enumeration.Add(topic);

        context.Document.Definitions["WebsocketTopics"] = schema;
    }
}
