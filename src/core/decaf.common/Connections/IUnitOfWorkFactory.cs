using System;
using System.Threading;
using System.Threading.Tasks;

namespace decaf.common.Connections
{
	public interface IUnitOfWorkFactory : IDisposable
	{
		/// <summary>
		/// Create a new unit of work, using connection details provided through the DI Container.
		/// </summary>
		/// <returns>A new <see cref="IUnitOfWork"/>.</returns>
		IUnitOfWork Create();
		
		/// <summary>
		/// Create a new unit of work, using provided connection details.
		/// </summary>
		/// <param name="connectionDetails">The details of the connection to use.</param>
		/// <returns>A new <see cref="IUnitOfWork"/>.</returns>
		IUnitOfWork Create(IConnectionDetails connectionDetails);

		/// <summary>
		/// (awaitable) Create a new unit of work, using connection details provided through the DI Container.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>A new <see cref="IUnitOfWork"/>.</returns>
		Task<IUnitOfWork> CreateAsync(CancellationToken cancellationToken = default);
		
		/// <summary>
		/// (awaitable) Create a new unit of work, using provided connection details.
		/// </summary>
		/// <param name="connectionDetails">The details of the connection to use.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>A new <see cref="IUnitOfWork"/>.</returns>
		Task<IUnitOfWork> CreateAsync(IConnectionDetails connectionDetails, CancellationToken cancellationToken = default);
	}

	internal interface IUnitOfWorkFactoryInternal : IUnitOfWorkFactory
    {
		/// <summary>
		/// Notify the transient factory that the transient has been disposed.
		/// </summary>
		/// <param name="id">The ID of the transient that was disposed.</param>
		void NotifyUnitOfWorkDisposed(Guid id);
	}
}

