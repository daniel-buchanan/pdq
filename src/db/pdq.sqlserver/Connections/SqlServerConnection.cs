﻿using System.Data;
using pdq.common.Connections;
using pdq.common.Logging;

namespace pdq.sqlserver
{
	public class SqlServerConnection : Connection, IConnection
	{
        public SqlServerConnection(
            ILoggerProxy logger,
            IConnectionDetails connectionDetails)
            : base(logger, connectionDetails)
        {
        }

        public override IDbConnection GetUnderlyingConnection()
        {
            var details = this.connectionDetails as ISqlServerConnectionDetails;
            return new Microsoft.Data.SqlClient.SqlConnection(details.GetConnectionString());
        }
    }
}
