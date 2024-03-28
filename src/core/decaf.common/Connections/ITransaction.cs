using System;
using System.Data;

namespace decaf.common.Connections
{
	public interface ITransaction : IDisposable
	{
		Guid Id { get; }

		IConnection Connection { get; }

		void Begin();

		void Commit();

		void Rollback();

		IDbTransaction GetUnderlyingTransaction();
		
		TransactionState State { get; }
	}

	internal interface ITransactionInternal : ITransaction
    {
		bool CloseConnectionOnCommitOrRollback { get; }
	}
}

