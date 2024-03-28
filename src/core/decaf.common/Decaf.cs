using System.Threading;
using System.Threading.Tasks;
using decaf.common.Connections;
using decaf.common.Exceptions;
using decaf.common.Logging;
using decaf.common.Utilities;

namespace decaf.common
{
    public class Decaf : IDecaf
	{
        private readonly ILoggerProxy logger;
        private readonly IUnitOfWorkFactory unitOfWorkFactory;
        private readonly IConnectionDetails injectedConnectionDetails;

		public Decaf(
            ILoggerProxy logger,
            IUnitOfWorkFactory unitOfWorkFactory)
		{
            this.logger = logger;
            this.unitOfWorkFactory = unitOfWorkFactory;
		}

        public Decaf(
            ILoggerProxy logger,
            IUnitOfWorkFactory unitOfWorkFactory,
            IConnectionDetails connectionDetails)
            : this(logger, unitOfWorkFactory)
        {
            this.injectedConnectionDetails = connectionDetails;
        }

        private IUnitOfWork GetUnitOfWork(IConnectionDetails connectionDetails = null)
            => GetUnitOfWorkAsync(connectionDetails).WaitFor();

        private async Task<IUnitOfWork> GetUnitOfWorkAsync(
            IConnectionDetails connectionDetails = null,
            CancellationToken cancellationToken = default)
        {
            if (this.injectedConnectionDetails == null &&
               connectionDetails == null)
            {
                this.logger.Error("ConnectionDetails could not be found, either not injected or provided to method");
                throw new MissingConnectionDetailsException("ConnectionDetails were not injected or provided. Please ensure that either method is used.");
            }

            IConnectionDetails connectionDetailsToUse;
            if (connectionDetails != null) connectionDetailsToUse = connectionDetails;
            else connectionDetailsToUse = this.injectedConnectionDetails;

            return await this.unitOfWorkFactory.CreateAsync(connectionDetailsToUse, cancellationToken);
        }

        /// <inheritdoc/>
        public IUnitOfWork BuildUnit()
            => GetUnitOfWork();

        /// <inheritdoc/>
        public IUnitOfWork BuildUnit(IConnectionDetails connectionDetails)
            => GetUnitOfWork(connectionDetails);

        /// <inheritdoc/>
        public Task<IUnitOfWork> BuildUnitAsync(CancellationToken cancellationToken = default)
            => GetUnitOfWorkAsync(cancellationToken: cancellationToken);

        /// <inheritdoc/>
        public async Task<IUnitOfWork> BuildUnitAsync(
            IConnectionDetails connectionDetails,
            CancellationToken cancellationToken = default)
            => await GetUnitOfWorkAsync(connectionDetails, cancellationToken);

        /// <inheritdoc/>
        public IQueryContainer Query()
            => GetUnitOfWork().Query();

        /// <inheritdoc/>
        public IQueryContainer Query(IConnectionDetails connectionDetails)
            => GetUnitOfWork(connectionDetails).Query();

        /// <inheritdoc/>
        public async Task<IQueryContainer> QueryAsync(CancellationToken cancellationToken = default)
            => await QueryAsync(null, cancellationToken);

        /// <inheritdoc/>
        public async Task<IQueryContainer> QueryAsync(
            IConnectionDetails connectionDetails,
            CancellationToken cancellationToken = default)
        {
            var t = await GetUnitOfWorkAsync(connectionDetails, cancellationToken);
            return await t.QueryAsync(cancellationToken);
        }
    }
}

