namespace DistLib;

public class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
    public DateTimeOffset UtcNowOffset => DateTimeOffset.UtcNow;
    public TimeSpan TimeSpanFromMinutes(int minutes) => TimeSpan.FromMinutes(minutes);
    
}