using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using pdq.common.Connections;
using pdq.common.Logging;
using pdq.common.Utilities;

namespace pdq.common
{
	public sealed class TransientFactory : ITransientFactory
	{
        private readonly List<ITransient> tracker;
        private readonly PdqOptions options;
        private readonly ILoggerProxy logger;
        private readonly ITransactionFactory transactionFactory;

        public TransientFactory(
            PdqOptions options,
            ILoggerProxy logger,
            ITransactionFactory transactionFactory)
		{
            this.tracker = new List<ITransient>();
            this.options = options;
            this.logger = logger;
            this.transactionFactory = transactionFactory;
		}

        /// <inheritdoc/>
        public ITransient Create(IConnectionDetails connectionDetails) => CreateAsync(connectionDetails).WaitFor();

        /// <inheritdoc/>
        public async Task<ITransient> CreateAsync(IConnectionDetails connectionDetails)
        {
            this.logger.Debug("TransientFactory :: Getting Transaction");
            var transaction = await this.transactionFactory.GetAsync(connectionDetails);
            var transient = Transient.Create(this, transaction, this.logger, this.options);
            this.logger.Debug($"TransientFactory :: Transient ({transient.Id}) Tracked");

            if(this.options.TrackTransients) this.tracker.Add(transient);

            return transient;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (!this.options.TrackTransients) return;

            this.logger.Debug($"TransientFactory :: Disposing Resources, {this.tracker.Count} Transients");
            tracker.Clear();
        }

        /// <inheritdoc/>
        void ITransientFactory.NotifyTransientDisposed(Guid id)
        {
            if (!this.options.TrackTransients) return;

            var transient = this.tracker.FirstOrDefault(t => t.Id == id);
            if (transient == null)
            {
                this.logger.Debug($"TransientFactory :: Notified Transient ({id}) disposed, but was not tracked");
                return;
            }

            this.logger.Debug($"TransientFactory :: Notified Transient({id}) disposed, tracking removed");
            this.tracker.Remove(transient);
        }
    }
}