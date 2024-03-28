using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using decaf.common.Logging;
using decaf.common.Utilities;

namespace decaf.common.Connections
{
    public sealed class UnitOfWork : IUnitOfWorkExtended
    {
        private readonly IConnection connection;
        private readonly ITransactionInternal transaction;
        private readonly Action<Guid> notifyDisposed;
        private readonly ISqlFactory sqlFactory;
        private readonly ILoggerProxy logger;
        private readonly DecafOptions options;
        private readonly IHashProvider hashProvider;
        private readonly List<IQueryContainer> queries;
        private Func<Exception, bool> catchHandler;
        private Action successHandler;

        private UnitOfWork(
            ITransaction transaction,
            ISqlFactory sqlFactory,
            ILoggerProxy logger,
            IHashProvider hashProvider,
            DecafOptions options,
            Action<Guid> notifyDisposed)
        {
            this.connection = transaction.Connection;
            this.transaction = transaction as ITransactionInternal;
            this.sqlFactory = sqlFactory;
            this.logger = logger;
            this.options = options;
            this.hashProvider = hashProvider;
            this.notifyDisposed = notifyDisposed;
            this.queries = new List<IQueryContainer>();

            Id = Guid.NewGuid();
            this.logger.Debug($"UnitOfWork({Id}) :: Created");
        }

        /// <inheritdoc />
        public Guid Id { get; private set; }

        /// <inheritdoc />
        IConnection IUnitOfWorkExtended.Connection => this.connection;

        /// <inheritdoc />
        ITransaction IUnitOfWorkExtended.Transaction => this.transaction;

        /// <inheritdoc />
        ISqlFactory IUnitOfWorkExtended.SqlFactory => this.sqlFactory;

        public static IUnitOfWork Create(
            ITransaction transaction,
            ISqlFactory sqlFactory,
            ILoggerProxy logger,
            IHashProvider hashProvider,
            DecafOptions options,
            Action<Guid> notifyDisposed)
            => new UnitOfWork(transaction, sqlFactory, logger, hashProvider, options, notifyDisposed);

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!disposing) return;
            if (Enumerable.Any(this.queries, q => q.Status != QueryStatus.Executed))
            {
                this.logger.Warning($"UnitOfWork({Id}) :: One or more queries have not been executed.");
            }

            if (this.transaction.State == TransactionState.Disposed ||
                this.transaction.State == TransactionState.RolledBack)
            {
                this.logger.Warning($"UnitOfWork({Id}) :: Unable to commit, transaction is already in {transaction.State} state.");
                return;
            }

            Persist();

            this.logger.Debug($"UnitOfWork({Id}) :: Disposed");
            this.notifyDisposed(this.Id);
        }

        private void Persist()
        {
            try
            {
                this.logger.Debug($"UnitOfWork({Id}) :: Committing Transaction");
                this.transaction.Commit();
                this.successHandler?.Invoke();
            }
            catch (Exception commitException)
            {
                this.logger.Error(commitException, $"UnitOfWork({Id}) :: Committing Transaction Failed");
                var reThrow = this.catchHandler?.Invoke(commitException);
                try
                {
                    this.logger.Debug($"UnitOfWork({Id}) :: Rolling back Transaction");
                    this.transaction.Rollback();
                    if (reThrow == true) throw;
                }
                catch (Exception rollbackException)
                {
                    this.logger.Error(rollbackException, $"UnitOfWork({Id}) :: Rolling back Transaction Failed");
                    reThrow = this.catchHandler?.Invoke(rollbackException);
                    if (reThrow == true) throw;
                }
            }
            finally
            {
                if (this.transaction.CloseConnectionOnCommitOrRollback)
                {
                    this.logger.Debug($"UnitOfWork({Id}) :: Closing Connection after Commit or Rollback");
                    this.connection.Close();
                }
            }
        }

        /// <inheritdoc />
        public IQueryContainer Query()
            => QueryAsync().WaitFor();

        /// <inheritdoc />
        public IUnitOfWork Query(Action<IQueryContainer> method)
        {
            var query = Query();
            method(query);
            return this;
        }

        /// <inheritdoc />
        public Task<IQueryContainer> QueryAsync(CancellationToken cancellationToken = default)
        {
            var query = QueryContainer.Create(this, this.logger, this.hashProvider, this.options);
            this.logger.Debug($"UnitOfWork({Id}) :: Creating new Query");
            this.queries.Add(query);
            return Task.FromResult(query);
        }

        /// <inheritdoc />
        public async Task<IUnitOfWork> QueryAsync(Func<IQueryContainer, Task> method, CancellationToken cancellationToken = default)
        {
            var query = await QueryAsync(cancellationToken);
            await method(query);
            return this;
        }

        /// <inheritdoc />
        public IUnitOfWork OnException(Action<Exception> handler)
        {
            catchHandler = e =>
            {
                handler(e);
                return false;
            };
            return this;
        }

        /// <inheritdoc />
        public IUnitOfWork OnException(Func<Exception, bool> handler)
        {
            catchHandler = handler;
            return this;
        }

        /// <inheritdoc />
        public Task<IUnitOfWork> OnExceptionAsync(Func<Exception, Task> handler, CancellationToken cancellationToken = default)
        {
            catchHandler = ex =>
            {
                handler(ex).WaitFor(cancellationToken);
                return false;
            };
            var uow = this as IUnitOfWork;
            return Task.FromResult(uow);
        }

        /// <inheritdoc />
        public Task<IUnitOfWork> OnExceptionAsync(Func<Exception, Task<bool>> handler, CancellationToken cancellationToken = default)
        {
            catchHandler = ex => handler(ex).WaitFor();
            return Task.FromResult<IUnitOfWork>(this);
        }

        /// <inheritdoc />
        public IUnitOfWork OnSuccess(Action handler)
        {
            successHandler = handler;
            return this;
        }

        /// <inheritdoc />
        public Task<IUnitOfWork> OnSuccessAsync(Func<Task> handler, CancellationToken cancellationToken = default)
        {
            successHandler = () => handler().WaitFor(cancellationToken);
            var uow = this as IUnitOfWork;
            return Task.FromResult(uow);
        }

        /// <inheritdoc />
        public IUnitOfWork PersistChanges()
        {
            Persist();
            return this;
        }

        /// <inheritdoc />
        public Task<IUnitOfWork> PersistChangesAsync(CancellationToken cancellationToken = default)
        {
            Persist();
            var uow = this as IUnitOfWork;
            return Task.FromResult(uow);
        }

        /// <inheritdoc />
        public void NotifyQueryDisposed(Guid queryId)
        {
            var found = this.queries.FirstOrDefault(q => q.Id == queryId);
            if (found == null)
            {
                this.logger.Debug($"UnitOfWork({Id}) :: Could not find query with Id - {queryId}");
                return;
            }

            this.queries.Remove(found);

            if (this.queries.Count != 0) return;

            this.Dispose();
        }
    }
}

