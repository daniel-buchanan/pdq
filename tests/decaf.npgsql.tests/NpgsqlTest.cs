using System;
using decaf.common.Connections;
using Microsoft.Extensions.DependencyInjection;

namespace decaf.npgsql.tests
{
	public abstract class NpgsqlTest
	{
        protected IServiceProvider provider;
        protected readonly IServiceCollection services;

		public NpgsqlTest(bool disableHeaderComments = true)
		{
            services = new ServiceCollection();
            services.AddDecaf(o =>
            {
                o.TrackUnitsOfWork()
                    .OverrideDefaultLogLevel(LogLevel.Debug)
                    .UseNpgsql(options =>
                    {
                        options.WithConnectionDetails(new NpgsqlConnectionDetails()
                        {
                            Authentication = new UsernamePasswordAuthentication("bob", "password")
                        });
                    });

                if(disableHeaderComments) o.DisableSqlHeaderComments();
            });
        }

        protected void BuildServiceProvider() => provider = services.BuildServiceProvider();
	}
}

