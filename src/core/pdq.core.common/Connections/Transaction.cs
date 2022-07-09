﻿using System;
using System.Data;
using pdq.common.Logging;

namespace pdq.common.Connections
{
	public abstract class Transaction : ITransaction
	{
        private readonly ILoggerProxy logger;
        protected readonly IConnection connection;
        protected IDbTransaction transaction;
        protected PdqOptions options;

		protected Transaction(
            Guid id,
            ILoggerProxy logger,
            IConnection connection,
            PdqOptions options)
		{
            Id = id;
            this.logger = logger;
            this.connection = connection;
            this.options = options;

            this.logger.Debug($"ITransaction({Id}) :: Transaction created");
		}

        /// <inheritdoc/>
        public IConnection Connection => this.connection;

        /// <inheritdoc/>
        public Guid Id { get; }

        /// <inheritdoc/>
        bool ITransaction.CloseConnectionOnCommitOrRollback => this.options.CloseConnectionOnCommitOrRollback;

        /// <inheritdoc/>
        public void Begin()
        {
            if (this.transaction != null) return;

            this.logger.Debug($"ITransaction({Id}) :: Beginning Transaction");
            this.transaction = GetUnderlyingTransaction();
        }

        /// <inheritdoc/>
        public void Commit()
        {
            try
            {
                this.logger.Debug($"ITransaction({Id}) :: Committing Transaction");
                this.transaction.Commit();
                this.logger.Debug($"ITransaction({Id}) :: Commit SUCCEEDED");
            }
            catch (Exception commitEx)
            {
                try
                {
                    this.logger.Error(commitEx, $"ITransaction({Id}) :: Commit FAILED, attempting Rollback");
                    this.transaction.Rollback();
                    this.logger.Information($"ITransaction({Id}) :: Rollback SUCCEEDED");
                }
                catch (Exception rollbackEx)
                {
                    this.logger.Error(rollbackEx, $"ITransaction({Id}) :: Rollback FAILED");
                }
            }
            finally
            {
                this.logger.Debug($"ITransaction({Id}) :: Finished Commit Process");
            }
        }

        /// <inheritdoc/>
        public void Dispose() => Dispose(true);

        protected void Dispose(bool disposing)
        {
            this.transaction = null;
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        public void Rollback()
        {
            try
            {
                this.logger.Debug($"ITransaction({Id}) :: Rolling back Transaction");
                this.transaction.Rollback();
                this.logger.Debug($"ITransaction({Id}) :: Rollback SUCCEEDED");
            }
            catch (Exception rollbackEx)
            {
                this.logger.Error(rollbackEx, $"ITransaction({Id}) :: Rollback FAILED");
            }
        }

        /// <inheritdoc/>
        public abstract IDbTransaction GetUnderlyingTransaction();
    }
}

