namespace Application.Interfaces.Infrastructure.MQTT;

public interface IDataValidator<T>
{
    bool IsDataComplete(T dto);
}