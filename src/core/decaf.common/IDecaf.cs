using System.Threading;
using System.Threading.Tasks;
using decaf.common.Connections;

namespace decaf.common
{
	public interface IDecaf
	{
        /// <summary>
        /// Begin a Query, using injected connection details
        /// </summary>
        /// <returns>An <see cref="IQueryContainer"/> ready to be used.</returns>
        IQueryContainer Query();

        /// <summary>
		/// Begin a Query, providing connection details
		/// </summary>
		/// <param name="connectionDetails">The <see cref="IConnectionDetails"/> to use.</param>
		/// <returns>An <see cref="IQueryContainer"/> ready to be used.</returns>
		IQueryContainer Query(IConnectionDetails connectionDetails);

        /// <summary>
        /// Begin a Query, using injected connection details (async)
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to use (optional).</param>
        /// <returns>An <see cref="IQueryContainer"/> ready to be used.</returns>
        Task<IQueryContainer> QueryAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Begin a Query, providing connection details (async)
        /// </summary>
        /// <param name="connectionDetails">The connection details to use.</param>
        /// <param name="cancellationToken">The cancellation token to use (optional).</param>
        /// <returns>A query ready to be used.</returns>
        Task<IQueryContainer> QueryAsync(IConnectionDetails connectionDetails, CancellationToken cancellationToken = default);

		/// <summary>
		/// Begin a <see cref="IUnitOfWork"/> allowing you to execute multiple queries.
		/// </summary>
		/// <returns>An <see cref="IUnitOfWork"/>.</returns>
		IUnitOfWork BuildUnit();

        /// <summary>
        /// Begin a <see cref="IUnitOfWork"/> allowing you to execute multiple queries.
        /// </summary>
        /// <param name="connectionDetails">The <see cref="IConnectionDetails"/> to use.</param>
        /// <returns>An <see cref="IUnitOfWork"/>.</returns>
        IUnitOfWork BuildUnit(IConnectionDetails connectionDetails);

        /// <summary>
		/// Begin a <see cref="IUnitOfWork"/> allowing you to execute multiple queries. (async)
		/// </summary>
        /// <param name="cancellationToken">The cancellation token to use (optional).</param>
		/// <returns>An <see cref="IUnitOfWork"/>.</returns>
		Task<IUnitOfWork> BuildUnitAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Begin a <see cref="IUnitOfWork"/> allowing you to execute multiple queries. (async)
        /// </summary>
        /// <param name="connectionDetails">The <see cref="IConnectionDetails"/> to use.</param>
        /// <param name="cancellationToken">The cancellation token to use (optional).</param>
        /// <returns>An <see cref="IUnitOfWork"/>.</returns>
		Task<IUnitOfWork> BuildUnitAsync(IConnectionDetails connectionDetails, CancellationToken cancellationToken = default);
	}
}

