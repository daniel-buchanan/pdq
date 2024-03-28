using System;
using decaf.common;
using decaf.common.Connections;
using decaf.tests.common.Mocks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace decaf.core_tests.Connections
{
    public class TransientTests
    {
        private readonly IDecaf decaf;

        public TransientTests()
        {
            var services = new ServiceCollection();
            services.AddDecaf(o =>
            {
                o.UseMockDatabase();
            });
            services.AddScoped<IConnectionDetails, MockConnectionDetails>();
            var provider = services.BuildServiceProvider();
            this.decaf = provider.GetService<IDecaf>();
        }

        [Fact]
        public void CanGetTransientSucceeds()
        {
            // Act
            Action method = () => this.decaf.BuildUnit();

            // Assert
            method.Should().NotThrow();
        }

        [Fact]
        public void NewTransientHasId()
        {
            // Act
            var transient = this.decaf.BuildUnit();

            // Assert
            transient.Should().NotBeNull();
            transient.Id.Should().NotBe(Guid.Empty);
        }

        [Fact]
        public void CanGetConnectionSucceeds()
        {
            // Arrange
            var transient = this.decaf.BuildUnit() as IUnitOfWorkExtended;

            // Act
            Func<IConnection> method = () => transient.Connection;

            // Assert
            method.Should().NotThrow();
            var result = method();
            result.Should().NotBeNull();
        }

        [Fact]
        public void CanGetTransactionSucceeds()
        {
            // Arrange
            var transient = this.decaf.BuildUnit() as IUnitOfWorkExtended;

            // Act
            Func<ITransaction> method = () => transient.Transaction;

            // Assert
            method.Should().NotThrow();
            var result = method();
            result.Should().NotBeNull();
        }

        [Fact]
        public void NotifyQueryDisposed_UnknownQueryDoesNothing()
        {
            // Arrange
            var transient = this.decaf.BuildUnit() as IUnitOfWorkExtended;

            // Act
            transient.NotifyQueryDisposed(Guid.Empty);
            var method = () => transient.Query();

            // Assert
            method.Should().NotThrow<ObjectDisposedException>();
        }
    }
}

