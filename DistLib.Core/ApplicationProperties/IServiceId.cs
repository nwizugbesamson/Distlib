namespace DistLib;

public interface IServiceId
{
    string Id { get; }
}

public class ServiceId : IServiceId
{
    public string Id { get; } = $"{Guid.NewGuid():N}";
}