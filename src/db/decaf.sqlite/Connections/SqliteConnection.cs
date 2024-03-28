using System.Data;
using System.Data.SQLite;
using decaf.common.Connections;
using decaf.common.Logging;

namespace decaf.sqlite;

public class SqliteConnection : Connection, IConnection
{
    /// <inheritdoc />
    public SqliteConnection(
        ILoggerProxy logger, 
        IConnectionDetails connectionDetails) : 
        base(logger, connectionDetails) { }

    public override IDbConnection GetUnderlyingConnection()
    {
        var details = this.connectionDetails as ISqliteConnectionDetails;
        return new SQLiteConnection(details.GetConnectionString());
    }
}