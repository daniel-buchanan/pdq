using decaf.common.Connections;
using decaf.common.Logging;

namespace decaf.sqlite;

public class SqliteConnectionFactory : ConnectionFactory
{
    public SqliteConnectionFactory(ILoggerProxy logger) : base(logger) { }

    protected override Task<IConnection> ConstructConnectionAsync(
        IConnectionDetails connectionDetails, 
        CancellationToken cancellationToken = default)
    {
        var conn = new SqliteConnection(this.logger, connectionDetails);
        return Task.FromResult(conn as IConnection);
    }
}