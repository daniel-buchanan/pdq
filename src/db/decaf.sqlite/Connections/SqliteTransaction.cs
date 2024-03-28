using System.Data;
using decaf.common.Connections;
using decaf.common.Exceptions;
using decaf.common.Logging;

namespace decaf.sqlite;

public class SqliteTransaction : Transaction
{
    public SqliteTransaction(
        Guid id, 
        ILoggerProxy logger, 
        IConnection connection, 
        DecafOptions options) : 
        base(id, logger, connection, options) { }

    public override IDbTransaction GetUnderlyingTransaction()
    {
        var sqliteConnection = this.connection as SqliteConnection;
        if (sqliteConnection == null)
            throw new InvalidConnectionException($"The provided connection for Transaction {this.Id} is not of the type \"NpgsqlConnection\"");

        return sqliteConnection.GetUnderlyingConnection()
            .BeginTransaction();
    }
}