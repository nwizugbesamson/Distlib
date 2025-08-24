namespace DistLib;

public interface IRollBackAction
{
    Task RollBackAsync(CancellationToken cancellationToken);
}