using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using decaf.common.Exceptions;
using decaf.common.Logging;
using decaf.common.Utilities;

namespace decaf.common.Connections
{
    public sealed class UnitOfWorkFactory : IUnitOfWorkFactoryInternal
	{
        private readonly List<IUnitOfWork> tracker;
        private readonly DecafOptions options;
        private readonly ILoggerProxy logger;
        private readonly ITransactionFactory transactionFactory;
        private readonly ISqlFactory sqlFactory;
        private readonly IHashProvider hashProvider;
        private readonly IConnectionDetails providedConnectionDetails;

        public UnitOfWorkFactory(
            DecafOptions options,
            ILoggerProxy logger,
            ITransactionFactory transactionFactory,
            ISqlFactory sqlFactory,
            IHashProvider hashProvider)
		{
            this.tracker = new List<IUnitOfWork>();
            this.options = options;
            this.logger = logger;
            this.transactionFactory = transactionFactory;
            this.sqlFactory = sqlFactory;
            this.hashProvider = hashProvider;
        }
        
        public UnitOfWorkFactory(
            DecafOptions options,
            ILoggerProxy logger,
            ITransactionFactory transactionFactory,
            ISqlFactory sqlFactory,
            IHashProvider hashProvider,
            IConnectionDetails connectionDetails) : 
            this(options, logger, transactionFactory, sqlFactory, hashProvider)
        {
            this.providedConnectionDetails = connectionDetails;
        }

        /// <inheritdoc/>
        public IUnitOfWork Create() => CreateAsync().WaitFor();

        /// <inheritdoc/>
        public IUnitOfWork Create(IConnectionDetails connectionDetails) => CreateAsync(connectionDetails).WaitFor();

        /// <inheritdoc/>
        public async Task<IUnitOfWork> CreateAsync(CancellationToken cancellationToken = default)
        {
            if (this.providedConnectionDetails == null)
                throw new MissingConnectionDetailsException("No connection details found.");
            return await CreateAsync(this.providedConnectionDetails, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<IUnitOfWork> CreateAsync(IConnectionDetails connectionDetails, CancellationToken cancellationToken = default)
        {
            this.logger.Debug("UnitOfWorkFactory :: Getting Transaction");
            var transaction = await this.transactionFactory.GetAsync(connectionDetails, cancellationToken);
            var uow = UnitOfWork.Create(
                transaction,
                this.sqlFactory,
                this.logger,
                this.hashProvider,
                this.options,
                NotifyUnitOfWorkDisposed);
            this.logger.Debug($"UnitOfWorkFactory :: UnitOfWork ({uow.Id}) Tracked");

            if(this.options.TrackUnitsOfWork) this.tracker.Add(uow);

            return uow;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (!this.options.TrackUnitsOfWork) return;

            this.logger.Debug($"UnitOfWorkFactory :: Disposing Resources, {this.tracker.Count} UnitOfWorks");
            tracker.Clear();
        }

        private void NotifyUnitOfWorkDisposed(Guid id)
        {
            if (!this.options.TrackUnitsOfWork) return;

            var uow = this.tracker.FirstOrDefault(t => t.Id == id);
            if (uow == null)
            {
                this.logger.Debug($"UnitOfWorkFactory :: Notified UnitOfWork ({id}) disposed, but was not tracked");
                return;
            }

            this.logger.Debug($"UnitOfWorkFactory :: Notified UnitOfWork({id}) disposed, tracking removed");
            this.tracker.Remove(uow);
        }

        /// <inheritdoc/>
        void IUnitOfWorkFactoryInternal.NotifyUnitOfWorkDisposed(Guid id)
            => this.NotifyUnitOfWorkDisposed(id);
    }
}