using System;
using System.Threading.Tasks;
using pdq.common.Utilities;

namespace pdq.common.Connections
{
	public abstract class ConnectionDetails : IConnectionDetails
	{
        private string connectionString;
        private string hostname;
        private int? port;
        private string databaseName;

		protected ConnectionDetails()
		{
            this.connectionString = null;
		}

        public string Hostname
        {
            get => this.hostname ?? String.Empty;
            protected set
            {
                if(!string.IsNullOrWhiteSpace(this.hostname))
                {
                    throw new ConnectionModificationException($"{nameof(Hostname)} cannot be modified once ConnectionDetails instance created");
                }

                this.hostname = value;
            }
        }

        public int Port
        {
            get => this.port ?? 0;
            protected set
            {
                if (port != null && port != 0)
                {
                    throw new ConnectionModificationException($"{nameof(Port)} cannot be modified once ConnectionDetails instance created");
                }

                this.port = value;
            }
        }

        public string DatabaseName
        {
            get => this.databaseName ?? String.Empty;
            protected set
            {
                if (!string.IsNullOrWhiteSpace(this.databaseName))
                {
                    throw new ConnectionModificationException($"{nameof(DatabaseName)} cannot be modified once ConnectionDetails instance created");
                }

                this.databaseName = value;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
            this.connectionString = null;
            this.hostname = null;
            this.port = null;
            this.databaseName = null;
        }

        public ValueTask DisposeAsync()
        {
            Dispose();
            return new ValueTask();
        }

        public string GetConnectionString() => GetConnectionStringAsync().WaitFor();

        public async Task<string> GetConnectionStringAsync()
        {
            if(this.connectionString != null)
            {
                return this.connectionString;
            }

            this.connectionString = await ConstructConnectionString();
            return this.connectionString;
        }

        protected abstract Task<string> ConstructConnectionString();

        string IConnectionDetails.GetHash() => GetConnectionString().ToBase64String();
    }
}

