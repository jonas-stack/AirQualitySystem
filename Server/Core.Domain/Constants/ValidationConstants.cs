namespace Core.Domain.Constants;

public static class ValidationConstants
{
    public const double MIN_TEMPERATURE = -100;
    public const double MAX_TEMPERATURE = 100;
    public const double MIN_HUMIDITY = 0;
    public const double MAX_HUMIDITY = 100;
    public const double MIN_AIR_QUALITY = 0;
    public const double MIN_PM25 = 0;
    public static readonly TimeSpan MAX_DATA_AGE = TimeSpan.FromHours(1);
    public static readonly TimeSpan MAX_FUTURE_OFFSET = TimeSpan.FromMinutes(5);
    
}
