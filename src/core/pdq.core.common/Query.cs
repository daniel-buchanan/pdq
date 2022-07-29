using System;
using pdq.common.Logging;

namespace pdq.common
{
	public class Query : IQueryInternal
	{
		private readonly ILoggerProxy logger;
		private readonly ITransient transient;
		private readonly IAliasManager aliasManager;
		private readonly PdqOptions options;
		private IQueryContext context;

		private Query(
			PdqOptions options,
			ILoggerProxy logger,
			ITransient transient)
		{
			this.logger = logger;
			this.transient = transient;
			this.aliasManager = AliasManager.Create();
			this.options = options;

			Id = Guid.NewGuid();
			Status = QueryStatus.Empty;

			this.logger.Debug($"Query({Id}) :: Created as {Status}");
		}

        public Guid Id { get; private set; }

		public QueryStatus Status { get; private set; }

		IAliasManager IQueryInternal.AliasManager => this.aliasManager;

        ITransient IQueryInternal.Transient => this.transient;

        IQueryContext IQueryInternal.Context => this.context;

        PdqOptions IQueryInternal.Options => this.options;

        string IQueryInternal.GetHash() => this.Id.ToString("N");

		public static IQuery Create(PdqOptions options, ILoggerProxy logger, ITransient transient)
			=> new Query(options, logger, transient);

		void IQueryInternal.SetContext(IQueryContext context) => this.context = context;

		public void Dispose()
		{
			Dispose(true);
            GC.SuppressFinalize(this);
        }

		protected virtual void Dispose(bool disposing)
        {
			if (!disposing) return;
			this.logger.Debug($"Query({Id} :: Disposing({disposing})");
            this.aliasManager.Dispose();
        }
    }
}

