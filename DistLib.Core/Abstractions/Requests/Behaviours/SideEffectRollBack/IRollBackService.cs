namespace DistLib;

public interface IRollBackService
{
    public IEnumerable<IRollBackAction> RollBackActions { get; }
    public void AddRollBackAction(IRollBackAction rollBackAction);
    public void ClearRollBackActions();
}