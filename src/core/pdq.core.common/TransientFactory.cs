﻿using System.Threading.Tasks;
using pdq.core.common.Connections;
using pdq.core.common.Logging;

namespace pdq.core.common
{
	public sealed class TransientFactory : ITransientFactory
	{
        private readonly ILoggerProxy logger;
        private readonly ITransactionFactory transactionFactory;

		public TransientFactory(
            ILoggerProxy logger,
            ITransactionFactory transactionFactory)
		{
            this.logger = logger;
            this.transactionFactory = transactionFactory;
		}

        public ITransient Create(IConnectionDetails connectionDetails)
        {
            var t = this.CreateAsync(connectionDetails);
            t.Wait();
            return t.Result;
        }

        public async Task<ITransient> CreateAsync(IConnectionDetails connectionDetails)
        {
            var transaction = await this.transactionFactory.GetAsync(connectionDetails);
            return new Transient(transaction, this.logger);
        }
    }
}