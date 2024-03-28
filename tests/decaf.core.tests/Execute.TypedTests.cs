using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using decaf.common;
using decaf.common.Connections;
using decaf.Implementation;
using decaf.state;
using decaf.tests.common.Mocks;
using decaf.tests.common.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace decaf.core_tests
{
    public class ExecuteTypedTests
    {
        private readonly IExecute<Person> execute;
        private readonly ISelectQueryContext context;

        public ExecuteTypedTests()
        {
            var services = new ServiceCollection();
            services.AddDecaf(o => o.UseMockDatabase());
            services.AddScoped<IConnectionDetails, MockConnectionDetails>();
            var provider = services.BuildServiceProvider();
            var decaf = provider.GetService<IDecaf>();
            var transient = decaf.BuildUnit();
            var query = transient.Query() as IQueryContainerInternal;
            var sqlFactory = provider.GetService<ISqlFactory>();
            this.context = query.CreateContext<ISelectQueryContext>();
            this.execute = new InheritedExecutor<ISelectQueryContext, Person>(query, this.context);
        }

        [Fact]
        public void DynamicReturnsSelf()
        {
            // Act
            var dyn = this.execute.Dynamic();

            // Assert
            dyn.Should().Be(this.execute);
        }

        [Fact]
        public void AsEnumerableDoesntThrow()
        {
            // Act
            Action method = () => this.execute.AsEnumerable();

            // Assert
            method.Should().NotThrow();
        }

        [Fact]
        public void AsEnumerableTypedDoesntThrow()
        {
            // Act
            Action method = () => this.execute.AsEnumerable();

            // Assert
            method.Should().NotThrow();
        }

        [Fact]
        public async Task AsEnumerableTypedAsyncDoesntThrow()
        {
            // Act
            Func<Task<IEnumerable<Person>>> method = () => this.execute.AsEnumerableAsync();

            // Assert
            await method.Should().NotThrowAsync();
        }

        [Fact]
        public void ToListDoesntThrow()
        {
            // Act
            Action method = () => this.execute.ToList();

            // Assert
            method.Should().NotThrow();
        }

        [Fact]
        public void ToListTypedDoesntThrow()
        {
            // Act
            Action method = () => this.execute.ToList();

            // Assert
            method.Should().NotThrow();
        }

        [Fact]
        public async Task ToListTypedAsyncDoesntThrow()
        {
            // Act
            Func<Task<IList<Person>>> method = () => this.execute.ToListAsync();

            // Assert
            await method.Should().NotThrowAsync();
        }

        [Fact]
        public void ExecuteDoesntThrow()
        {
            // Act
            Action method = () => this.execute.Execute();

            // Assert
            method.Should().NotThrow();
        }

        [Fact]
        public async Task ExecuteAsyncDoesntThrow()
        {
            // Act
            Func<Task> method = () => this.execute.ExecuteAsync();

            // Assert
            await method.Should().NotThrowAsync();
        }

        [Fact]
        public void FirstDoesThrow()
        {
            // Act
            Action method = () => this.execute.First();

            // Assert
            method.Should().Throw<Exception>();
        }

        [Fact]
        public async Task FirstAsyncDoesThrow()
        {
            // Act
            Func<Task> method = () => this.execute.FirstAsync();

            // Assert
            await method.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public void FirstTypedDoesThrow()
        {
            // Act
            Action method = () => this.execute.First();

            // Assert
            method.Should().Throw<Exception>();
        }

        [Fact]
        public async Task FirstTypedAsyncDoesThrow()
        {
            // Act
            Func<Task> method = () => this.execute.FirstAsync();

            // Assert
            await method.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public void FirstOrDefaultDoesntThrow()
        {
            // Act
            Action method = () => this.execute.FirstOrDefault();

            // Assert
            method.Should().NotThrow();
        }

        [Fact]
        public async Task FirstOrDefaultAsyncDoesntThrow()
        {
            // Act
            Func<Task> method = () => this.execute.FirstOrDefaultAsync();

            // Assert
            await method.Should().NotThrowAsync();
        }

        [Fact]
        public void FirstOrDefaultTypedDoesntThrow()
        {
            // Act
            Action method = () => this.execute.FirstOrDefault();

            // Assert
            method.Should().NotThrow();
        }

        [Fact]
        public async Task FirstOrDefaultTypedAsyncDoesntThrow()
        {
            // Act
            Func<Task> method = () => this.execute.FirstOrDefaultAsync();

            // Assert
            await method.Should().NotThrowAsync();
        }

        [Fact]
        public void SingleDoesThrow()
        {
            // Act
            Action method = () => this.execute.Single();

            // Assert
            method.Should().Throw<Exception>();
        }

        [Fact]
        public async Task SingleAsyncDoesThrow()
        {
            // Act
            Func<Task> method = () => this.execute.SingleAsync();

            // Assert
            await method.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public void SingleTypedDoesThrow()
        {
            // Act
            Action method = () => this.execute.Single();

            // Assert
            method.Should().Throw<Exception>();
        }

        [Fact]
        public async Task SingleTypedAsyncDoesThrow()
        {
            // Act
            Func<Task> method = () => this.execute.SingleAsync();

            // Assert
            await method.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public void SingleOrDefaultDoesntThrow()
        {
            // Act
            Action method = () => this.execute.SingleOrDefault();

            // Assert
            method.Should().NotThrow();
        }

        [Fact]
        public async Task SingleOrDefaultAsyncDoesntThrow()
        {
            // Act
            Func<Task> method = () => this.execute.SingleOrDefaultAsync();

            // Assert
            await method.Should().NotThrowAsync();
        }

        [Fact]
        public void SingleOrDefaultTypedDoesntThrow()
        {
            // Act
            Action method = () => this.execute.SingleOrDefault();

            // Assert
            method.Should().NotThrow();
        }

        [Fact]
        public async Task SingleOrDefaultTypedAsyncDoesntThrow()
        {
            // Act
            Func<Task> method = () => this.execute.SingleOrDefaultAsync();

            // Assert
            await method.Should().NotThrowAsync();
        }
    }

    class InheritedExecutor<TContext, TResult> : Execute<TResult, TContext>
        where TContext : IQueryContext
    {
        public InheritedExecutor(IQueryContainerInternal query, TContext context)
            : base(query, context)
        {
        }
    }
}

