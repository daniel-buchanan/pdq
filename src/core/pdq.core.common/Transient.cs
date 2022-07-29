using System;
using System.Collections.Generic;
using System.Linq;
using pdq.common.Connections;
using pdq.common.Logging;

namespace pdq.common
{
	public class Transient : ITransient
	{
        private readonly IConnection connection;
        private readonly ITransaction transaction;
        private readonly ITransientFactory factory;
        private readonly ILoggerProxy logger;
        private readonly PdqOptions options;
        private readonly List<IQuery> queries;

		private Transient(
            ITransientFactory factory,
            ITransaction transaction,
            ILoggerProxy logger,
            PdqOptions options)
		{
            this.factory = factory;
            this.connection = transaction.Connection;
            this.transaction = transaction;
            this.logger = logger;
            this.options = options;
            this.queries = new List<IQuery>();

            Id = Guid.NewGuid();
            this.logger.Debug($"Transient({Id}) :: Created");
		}

        /// <inheritdoc />
        public Guid Id { get; private set; }

        /// <inheritdoc />
        IConnection ITransient.Connection => this.connection;

        /// <inheritdoc />
        ITransaction ITransient.Transaction => this.transaction;

        public static ITransient Create(
            ITransientFactory factory,
            ITransaction transaction,
            ILoggerProxy logger,
            PdqOptions options)
            => new Transient(factory, transaction, logger, options);

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
            if (this.queries.Any(q => q.Status != QueryStatus.Executed))
            {
                this.logger.Warning($"Transient({Id}) :: One or more queries have not been executed.");
            }

            try
            {
                this.logger.Debug($"Transient({Id}) :: Committing Transaction");
                this.transaction.Commit();
            }
            catch (Exception commitException)
            {
                this.logger.Error(commitException, $"Transient({Id}) :: Committing Transaction Failed");
                try
                {
                    this.logger.Debug($"Transient({Id}) :: Rolling back Transaction");
                    this.transaction.Rollback();
                }
                catch (Exception rollbackException)
                {
                    this.logger.Error(rollbackException, $"Transient({Id}) :: Rolling back Transaction Failed");
                }
            }
            finally
            {
                if (this.transaction.CloseConnectionOnCommitOrRollback)
                {
                    this.logger.Debug($"Transient({Id}) :: Closing Connection after Commit or Rollback");
                    this.connection.Close();
                }
            }

            this.logger.Debug($"Transient({Id}) :: Disposed");
            this.factory.NotifyTransientDisposed(this.Id);
        }

        /// <inheritdoc />
        public IQuery Query()
        {
            var query = common.Query.Create(this.options, logger, this);
            this.logger.Debug($"Transient({Id}) :: Creating new Query");
            this.queries.Add(query);
            return query;
        }
    }
}

