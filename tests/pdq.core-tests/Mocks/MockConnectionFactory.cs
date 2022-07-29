using System;
using System.Threading.Tasks;
using pdq.common;
using pdq.common.Connections;
using pdq.common.Logging;

namespace pdq.core_tests.Mocks
{
    public class MockConnectionFactory : ConnectionFactory
    {
        public MockConnectionFactory(ILoggerProxy logger) : base(logger)
        {
        }

        protected override Task<IConnection> ConstructConnection(IConnectionDetails connectionDetails)
        {
            var connection = (IConnection)new MockConnection(this.logger, connectionDetails);
            return Task.FromResult(connection);
        }
    }
}

