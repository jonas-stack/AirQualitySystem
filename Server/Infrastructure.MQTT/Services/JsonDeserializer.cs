// Path: Server/Infrastructure.MQTT/MqttMessageDeserializer.cs

using System.Text;
using System.Text.Json;
using Application.Interfaces.Infrastructure.MQTT;
using Application.Models.Results;
using Microsoft.Extensions.Logging;

namespace Infrastructure.MQTT.Services;

public class JsonDeserializer : IJsonDeserializer
{
    private readonly ILogger<JsonDeserializer> _logger;

    public JsonDeserializer(ILogger<JsonDeserializer> logger)
    {
        _logger = logger;
    }

    public DeserializationResult<T> Deserialize<T>(byte[]? payload) where T : class
    {
        if (payload == null || payload.Length == 0)
        {
            return DeserializationResult<T>.Failed("Empty payload", string.Empty);
        }

        string json = Encoding.UTF8.GetString(payload);

        if (string.IsNullOrWhiteSpace(json))
        {
            return DeserializationResult<T>.Failed("Empty or whitespace JSON", json);
        }

        try
        {
            // Check if JSON starts with { and ends with }
            if (!(json.TrimStart().StartsWith("{") && json.TrimEnd().EndsWith("}")))
            {
                // Try to fix common issue of missing braces
                json = "{" + json + "}";
            }

            var dto = JsonSerializer.Deserialize<T>(json);

            if (dto == null)
            {
                _logger.LogDebug("JSON deserialized to null: {Json}", json);
                return DeserializationResult<T>.Failed("Deserialized to null", json);
            }

            return DeserializationResult<T>.Successful(dto, json);
        }
        catch (JsonException ex)
        {
            _logger.LogError("JSON deserialization failed: {Error}. Content: {Json}", 
                ex.Message, json);
            return DeserializationResult<T>.Failed($"Invalid JSON: {ex.Message}", json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error deserializing JSON");
            return DeserializationResult<T>.Failed($"Deserialization error: {ex.Message}", json);
        }
    }
}