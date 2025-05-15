using Application.Models.Results;

namespace Application.Interfaces.Infrastructure.MQTT;

public interface IMqttMessageDeserializer
{
    DeserializationResult<T> Deserialize<T>(byte[]? payload) where T : class;
}