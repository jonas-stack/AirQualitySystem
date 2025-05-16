namespace Application.Interfaces.Infrastructure.MQTT;

public interface IDataValidator
{
    bool IsDataComplete<T>(T dto) where T : class;
    bool IsIdValid<T>(T dto) where T : class;
    bool IsTimeStampValid<T>(T dto) where T : class;
}