using System;
using decaf.common;
using decaf.common.Options;
using decaf.db.common;

namespace decaf.sqlite
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Use SQLite (System.Data.Sqlite as the client) with the default configuration and options.
        /// </summary>
        /// <param name="services">The <see cref="IDecafOrmServiceCollection"/> to use.</param>
        public static IDecafOrmServiceCollection UseSqlite(this IDecafOrmServiceCollection services)
            => UseSqlite(services, new SqliteOptions());

        /// <summary>
        /// Use SQLite (System.Data.Sqlite as the client) with a builder for the SQLite options.
        /// </summary>
        /// <param name="services">The <see cref="IDecafOrmServiceCollection"/> to use.</param>
        /// <param name="builder">An <see cref="Action"/> to configure the <see cref="SqliteOptions"/>.</param>
        public static IDecafOrmServiceCollection UseSqlite(
            this IDecafOrmServiceCollection services,
            Action<ISqliteOptionsBuilder> builder)
        {
            var options = new SqliteOptionsBuilder();
            builder(options);
            return UseSqlite(services, options.Build());
        }

        /// <summary>
        /// Use SQLite (System.Data.Sqlite as the client) with a provided <see cref="SqliteOptions"/>.
        /// </summary>
        /// <param name="services">The <see cref="IDecafOptionsBuilder"/> to use.</param>
        /// <param name="options">The <see cref="SqliteOptions"/> to use.</param>
        public static IDecafOrmServiceCollection UseSqlite(
            this IDecafOrmServiceCollection services,
            SqliteOptions options)
        {
            services.UseDbImplementation<SqliteImplementationFactory>(options);
            return services;
        }
    }
}