namespace DistLib;

public interface IDateTimeProvider
{
    /// <summary>
    /// Gets the current date and time in UTC format.
    /// </summary>
    DateTime UtcNow { get; }

    DateTimeOffset UtcNowOffset { get; }

    TimeSpan TimeSpanFromMinutes(int minutes);
}