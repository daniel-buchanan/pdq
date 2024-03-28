using System;
using System.Reflection.Metadata;
using decaf.common.Connections;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using decaf.tests.common.Models;
using Xunit;

namespace decaf.sqlserver.tests
{
	public class SqlServerSetupTests
	{
        protected IServiceProvider provider;
        protected readonly IServiceCollection services;

		public SqlServerSetupTests()
		{
            services = new ServiceCollection();
            services.AddDecaf(o =>
            {
                o.TrackUnitsOfWork()
                    .OverrideDefaultLogLevel(LogLevel.Debug)
                    .UseSqlServer(options =>
                    {
                        options.WithConnectionDetails(new SqlServerConnectionDetails()
                        {
                            Authentication = new UsernamePasswordAuthentication("bob", "password")
                        });
                    });
            });
        }

        protected void BuildServiceProvider() => provider = services.BuildServiceProvider();

        [Fact]
        public void DefaultPortReturned()
        {
            // Arrange
            var details = new SqlServerConnectionDetails();

            // Assert
            details.Port.Should().Be(1433);
        }

        [Fact]
        public void CannotSetPortAfterGettingDefaultValue()
        {
            // Arrange
            var details = new SqlServerConnectionDetails();

            // Act
            Action method = () => details.Port = 1234;

            // Assert
            details.Port.Should().Be(1433);
            method.Should().Throw<ConnectionModificationException>();
        }

        [Fact]
        public void CanEnableTrustedConnection()
        {
            // Arrange
            var details = new SqlServerConnectionDetails();

            // Act
            details.Hostname = "localhost";
            details.DatabaseName = "db";
            details.IsTrustedConnection();

            // Assert
            details.GetConnectionString().Should().Be("Server=localhost,1433;Database=db;Trusted_Connection=Yes;");
        }

        [Fact]
        public void CanEnableMARS()
        {
            // Arrange
            var details = new SqlServerConnectionDetails();

            // Act
            details.Hostname = "localhost";
            details.DatabaseName = "db";
            details.IsTrustedConnection();
            details.EnableMars();

            // Assert
            details.GetConnectionString().Should().Be("Server=localhost,1433;Database=db;Trusted_Connection=Yes;MultipleActiveResultSets=True;");
        }

        [Fact]
        public void UsernamePasswordAuthCorrect()
        {
            // Arrange
            var details = new SqlServerConnectionDetails();

            // Act
            details.Hostname = "localhost";
            details.DatabaseName = "db";
            details.Authentication = new UsernamePasswordAuthentication("bob", "password:1");

            // Assert
            details.GetConnectionString().Should().Be("Server=localhost,1433;Database=db;User ID=bob;Password=password:1;");
        }
    }
}

