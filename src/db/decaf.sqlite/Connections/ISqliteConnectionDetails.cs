using decaf.common.Connections;

namespace decaf.sqlite;

public interface ISqliteConnectionDetails : IConnectionDetails
{
    string? FullUri { get; }
    
    bool InMemory { get; }
    
    bool CreateNew { get; }
    
    decimal Version { get; }
}