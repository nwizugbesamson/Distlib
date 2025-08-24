namespace DistLib;

public interface IEnvironmentProvider
{
    public string EnvironmentName { get;  }
    /// <summary>
    /// Determines whether the current environment is Development.
    /// </summary>
    /// <returns>True if the current environment is Development, false otherwise.</returns>
    bool IsDevelopment();

    public string NewLine { get; }
}