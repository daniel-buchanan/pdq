using System;
using System.Data;
using Moq;
using pdq.common.Connections;
using pdq.common.Logging;

namespace pdq.core_tests.Mocks
{
    public class MockTransaction : Transaction
    {
        public MockTransaction(
            Guid id,
            ILoggerProxy logger,
            IConnection connection,
            PdqOptions options)
            : base(id, logger, connection, options)
        {
        }

        public override IDbTransaction GetUnderlyingTransaction()
        {
            var transaction = new Mock<IDbTransaction>();
            return transaction.Object;
        }
    }
}

