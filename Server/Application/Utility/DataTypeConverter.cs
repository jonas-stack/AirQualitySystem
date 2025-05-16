namespace Application.Utility;

public class DataTypeConverter
{
    public static Guid GetDeviceGuid(string deviceId)
    {
        if (Guid.TryParse(deviceId, out Guid deviceGuid))
            return deviceGuid;
            
        return CreateDeterministicGuid(deviceId);
    }

    public static DateTime GetLocalDateTime(long timestampUnix)
    {
        var utcTime = DateTimeOffset.FromUnixTimeSeconds(timestampUnix).DateTime;
        return TimeZoneInfo.ConvertTimeFromUtc(utcTime,
            TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time"));
    }
    
    private static Guid CreateDeterministicGuid(string deviceId)
    {
        using (var md5 = System.Security.Cryptography.MD5.Create())
        {
            byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(deviceId);
            byte[] hashBytes = md5.ComputeHash(inputBytes);
            return new Guid(hashBytes);
        }
    }
}