using System;
using System.Threading;
using System.Threading.Tasks;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("decaf.services")]
namespace decaf.common.Connections
{
    /// <summary>
    /// 
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Gets the Id of the "Transient", this is unqiuely generated for each transient
        /// at the time that it is created and cannot be changed.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Begin a query on this "Transient", this will allow you to choose what
        /// type of query you wish to write and then execute.
        /// </summary>
        /// <returns>(FluentApi) The ability to begin, write and execute a query.</returns>
        /// <example>t.Query()</example>
        IQueryContainer Query();

        /// <summary>
        /// Execute a query on this "Unit of Work" using the Fluent API
        /// </summary>
        /// <param name="method">The method containing the query to run.</param>
        /// <returns>(FluentApi) A reference to this UnitOfWork</returns>
        IUnitOfWork Query(Action<IQueryContainer> method);

        /// <summary>
        /// (awaitable) Get a query so you can start building.
        /// </summary>
        /// <returns>A new query, from which you can choose how you create the sql statement.</returns>
        Task<IQueryContainer> QueryAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// (awaitable) Execute a query on this "Unit of Work" using the Fluent API
        /// </summary>
        /// <param name="method">The method containing the query to run.</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>(FluentApi) A reference to this UnitOfWork</returns>
        Task<IUnitOfWork> QueryAsync(Func<IQueryContainer, Task> method, CancellationToken cancellationToken = default);

        /// <summary>
        /// Provide a error handler function for commit failures.
        /// </summary>
        /// <param name="handler">A function to handle commit or rollback errors.</param>
        /// <returns>(FluentApi) A reference to this UnitOfWork.</returns>
        IUnitOfWork OnException(Action<Exception> handler);

        /// <summary>
        /// Provide a error handler function for commit failures.
        /// </summary>
        /// <param name="handler">A function to handle commit or rollback errors.</param>
        /// <returns>(FluentApi) A reference to this UnitOfWork.</returns>
        IUnitOfWork OnException(Func<Exception, bool> handler);

        /// <summary>
        /// (awaitable) Provide a error handler function for commit failures.
        /// </summary>
        /// <param name="handler">A function to handle commit or rollback errors.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>(FluentApi) A reference to this UnitOfWork.</returns>
        Task<IUnitOfWork> OnExceptionAsync(Func<Exception, Task> handler, CancellationToken cancellationToken = default);

        /// <summary>
        /// (awaitable) Provide a error handler function for commit failures.
        /// </summary>
        /// <param name="handler">A function to handle commit or rollback errors.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>(FluentApi) A reference to this UnitOfWork.</returns>
        Task<IUnitOfWork> OnExceptionAsync(Func<Exception, Task<bool>> handler, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Provide a success handler.
        /// </summary>
        /// <param name="handler">A function to handle successful commits.</param>
        /// <returns>(FluentApi) A reference to this UnitOfWork.</returns>
        IUnitOfWork OnSuccess(Action handler);

        /// <summary>
        /// (awaitable) Provide a success handler.
        /// </summary>
        /// <param name="handler">A function to handle successful commits.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>(FluentApi) A reference to this UnitOfWork.</returns>
        Task<IUnitOfWork> OnSuccessAsync(Func<Task> handler, CancellationToken cancellationToken = default);

        /// <summary>
        /// Attempt to execute any queries run on this Unit of Work.
        /// </summary>
        /// <returns>(FluentApi) A reference to this UnitOfWork.</returns>
        IUnitOfWork PersistChanges();

        /// <summary>
        /// (awaitable) Attempt to execute any queries run on this Unit of Work.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>(FluentApi) A reference to this UnitOfWork.</returns>
        Task<IUnitOfWork> PersistChangesAsync(CancellationToken cancellationToken = default);
    }

    public interface IUnitOfWorkExtended : IUnitOfWork
    {
        /// <summary>
        /// Gets the connection associated with this Transient.
        /// </summary>
        IConnection Connection { get; }

        /// <summary>
        /// Gets the transaction associated with this Transient.
        /// </summary>
        ITransaction Transaction { get; }

        /// <summary>
        /// Gets the SQL Factory for this query.
        /// </summary>
        ISqlFactory SqlFactory { get; }

        /// <summary>
        /// Notify the transient that a query has been disposed.
        /// </summary>
        /// <param name="queryId">The ID of the query that was disposed.</param>
        void NotifyQueryDisposed(Guid queryId);
    }
}