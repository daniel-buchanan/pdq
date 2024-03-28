using decaf.common.Connections;
using Microsoft.Extensions.DependencyInjection;

namespace decaf.common.Options
{
    public interface IDecafOptionsBuilder : IOptionsBuilder<DecafOptions>
    {
        /// <summary>
        /// Override the default log level (<see cref="LogLevel.Error"/>), and set
        /// to one of the available options.
        /// </summary>
        /// <param name="level">
        /// The log level that you want to set, you should not require a lower
        /// log level than "<see cref="LogLevel.Error"/>" for most use cases,
        /// although "<see cref="LogLevel.Information"/>" may also
        /// be useful.
        /// </param>
        /// <returns>(Fluent API) The ability to continnue further setup actions.</returns>
        IDecafOptionsBuilder OverrideDefaultLogLevel(LogLevel level);

        /// <summary>
        /// Override the default where clause handling behaviour (<see cref="ClauseHandling.And"/>), and
        /// set to on eof the available options.
        /// </summary>
        /// <param name="handling">
        /// The default where clause handling you want to use.
        /// </param>
        /// <returns>(Fluent API) The ability to continnue further setup actions.</returns>
        IDecafOptionsBuilder OverrideDefaultClauseHandling(ClauseHandling handling);

        /// <summary>
        /// Enable tracking of <see cref="IUnitOfWork"/> throughout their lifetime. By default
        /// they are not tracked, and unless you explicitly dispose of them they
        /// may not be disposed of until the GC collects them. However if you enable
        /// tracking they are tracked by the Unit of Work allowing for debugging and
        /// discovery of issues.
        /// </summary>
        /// <returns>(Fluent API) The ability to continue further setup actions.</returns>
        IDecafOptionsBuilder TrackUnitsOfWork();

        /// <summary>
        /// Close database connections after a commit or rollback. This will mean
        /// that any time a query is made it will first have to create and open
        /// a new connection to the database. If not enabled (default) then
        /// connections will be re-used for multiple queries across a transaction.
        /// </summary>
        /// <returns>(Fluent API) The ability to continue further setup actions.</returns>
        IDecafOptionsBuilder CloseConnectionsOnCommitOrRollback();

        /// <summary>
        /// Disables header comments for queries generated.
        /// This means primarily that the query hash and timestamp will not
        /// be included as comments at the beginning of the query.
        /// </summary>
        /// <returns>(Fluent API) The ability to continue further setup actions.</returns>
        IDecafOptionsBuilder DisableSqlHeaderComments();

        /// <summary>
        /// Inject the <see cref="IUnitOfWork"/> as a scoped service.
        /// </summary>
        /// <returns>(Fluent API) The ability to continue further setup actions.</returns>
        IDecafOptionsBuilder InjectUnitOfWorkAsScoped();

        /// <summary>
        /// Inject the <see cref="IUnitOfWork"/> using the provided lifetime
        /// </summary>
        /// <param name="lifetime">The lifetime for the <see cref="IUnitOfWork"/> when injected.</param>
        /// <returns>(FluentAPI) The ability to continue further setup actions.</returns>
        IDecafOptionsBuilder InjectUnitOfWork(ServiceLifetime lifetime);
    }

    public interface IDecafOptionsBuilderExtensions : IDecafOptionsBuilder
    {
        /// <summary>
        /// The Services Collection.
        /// </summary>
        IServiceCollection Services { get; }
    }
}

