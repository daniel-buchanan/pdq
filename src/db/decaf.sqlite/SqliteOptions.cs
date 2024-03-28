using decaf.db.common;

namespace decaf.sqlite;

public class SqliteOptions : DatabaseOptions
{
    public string DatabasePath { get; private set; }
    
    public bool InMemory { get; private set; }
    
    public bool CreateNew { get; private set; }

    public decimal Version { get; private set; } = 3;
    
}