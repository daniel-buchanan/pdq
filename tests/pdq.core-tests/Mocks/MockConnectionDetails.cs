using System;
using System.Threading.Tasks;
using pdq.common.Connections;

namespace pdq.core_tests.Mocks
{
	public class MockConnectionDetails : ConnectionDetails
	{
        protected override Task<string> ConstructConnectionString()
        {
            var connStr = $"{Hostname}:{Port},{DatabaseName}";
            return Task.FromResult(connStr);
        }
    }
}

