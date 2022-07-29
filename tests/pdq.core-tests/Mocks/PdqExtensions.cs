using System;
using pdq.core_tests.Mocks;

namespace pdq.core_tests.Mocks
{
	public static class PdqExtensions
	{
		public static void UseMockDatabase(this PdqOptions options)
        {
			options.ConnectionFactoryType = typeof(MockConnectionFactory);
			options.TransactionFactoryType = typeof(MockTransactionFactory);
        }
	}
}

