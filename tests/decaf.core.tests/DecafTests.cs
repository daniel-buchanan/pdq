using decaf.common;
using decaf.common.Exceptions;
using decaf.tests.common.Mocks;
using Xunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace decaf.core_tests
{
    public class DecafTests
    {
        static IDecaf GetDecaf(bool needsConnectionDetails = false)
        {
            var services = new ServiceCollection();
            services.AddDecaf(b =>
            {
                b.InjectUnitOfWorkAsScoped();
                b.UseMockDatabase();
                if (needsConnectionDetails) b.WithMockConnectionDetails();
            });
            var provider = services.BuildServiceProvider();
            return provider.GetRequiredService<IDecaf>();
        }

        [Fact]
        public void Begin_MissingConnectionDetailsThrows()
        {
            // Arrange
            var decaf = GetDecaf();

            // Act
            var method = () => decaf.BuildUnit();

            // Assert
            method.Should().Throw<MissingConnectionDetailsException>();
        }

        [Fact]
        public void BeginQuery_MissingConnectionDetailsThrows()
        {
            // Arrange
            var decaf = GetDecaf();

            // Act
            var method = () => decaf.Query();

            // Assert
            method.Should().Throw<MissingConnectionDetailsException>();
        }

        [Fact]
        public void BeginAsync_MissingConnectionDetailsThrows()
        {
            // Arrange
            var decaf = GetDecaf();

            // Act
            var method = () => decaf.BuildUnitAsync();

            // Assert
            method.Should().ThrowAsync<MissingConnectionDetailsException>();
        }

        [Fact]
        public void BeginQueryAsync_MissingConnectionDetailsThrows()
        {
            // Arrange
            var decaf = GetDecaf();

            // Act
            var method = () => decaf.QueryAsync();

            // Assert
            method.Should().ThrowAsync<MissingConnectionDetailsException>();
        }

        [Fact]
        public void Begin_ProvidedConnectionDetailsDoesNotThrow()
        {
            // Arrange
            var decaf = GetDecaf();
            var connectionDetails = new MockConnectionDetails();

            // Act
            var method = () => decaf.BuildUnit(connectionDetails);

            // Assert
            method.Should().NotThrow<MissingConnectionDetailsException>();
        }

        [Fact]
        public void BeginQuery_ProvidedConnectionDetailsDoesNotThrow()
        {
            // Arrange
            var decaf = GetDecaf();
            var connectionDetails = new MockConnectionDetails();

            // Act
            var method = () => decaf.Query(connectionDetails);

            // Assert
            method.Should().NotThrow<MissingConnectionDetailsException>();
        }

        [Fact]
        public void BeginAsync_ProvidedConnectionDetailsDoesNotThrow()
        {
            // Arrange
            var decaf = GetDecaf();
            var connectionDetails = new MockConnectionDetails();

            // Act
            var method = () => decaf.BuildUnitAsync(connectionDetails);

            // Assert
            method.Should().NotThrowAsync<MissingConnectionDetailsException>();
        }

        [Fact]
        public void BeginQueryAsync_ProvidedConnectionDetailsDoesNotThrow()
        {
            // Arrange
            var decaf = GetDecaf();
            var connectionDetails = new MockConnectionDetails();

            // Act
            var method = () => decaf.QueryAsync(connectionDetails);

            // Assert
            method.Should().NotThrowAsync<MissingConnectionDetailsException>();
        }
    }
}