using System;
using decaf.common;
using decaf.state;
using decaf.state.Conditionals;
using decaf.tests.common.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using decaf.db.common.Builders;
using decaf.tests.common.Mocks;
using Xunit;
using SelectBuilderPipeline = decaf.db.common.ANSISQL.SelectBuilderPipeline;

namespace decaf.services.tests.Query
{
    public class QueryServiceTests
    {
        private readonly IService<Person> personService;

        public QueryServiceTests()
        {
            var services = new ServiceCollection();
            services.AddDecaf(o =>
            {
                o.TrackUnitsOfWork();
                o.OverrideDefaultLogLevel(LogLevel.Debug);
                o.UseMockDatabase().WithMockConnectionDetails();
            });
            services.AddDecafService<Person>().AsScoped();

            var provider = services.BuildServiceProvider();
            this.personService = provider.GetService<IService<Person>>();
        }

        [Fact]
        public void GetAllDoesNotThrow()
        {
            // Arrange

            // Act
            Action method = () =>
            {
                this.personService.All();
            };

            // Assert
            method.Should().NotThrow();
        }

        [Fact]
        public void AllContextIsCorrect()
        {
            // Arrange
            IQueryContext context = null;
            this.personService.OnBeforeExecution += (sender, args) =>
            {
                context = args.Context;
            };

            // Act
            this.personService.All();

            // Assert
            context.Should().NotBeNull();
            var selectContext = context as ISelectQueryContext;
            selectContext.QueryTargets.Should().HaveCount(1);
            selectContext.Columns.Should().Satisfy(
                c => c.Name.Equals(nameof(Person.Id)),
                c => c.Name.Equals(nameof(Person.FirstName)),
                c => c.Name.Equals(nameof(Person.LastName)),
                c => c.Name.Equals(nameof(Person.Email)),
                c => c.Name.Equals(nameof(Person.CreatedAt)),
                c => c.Name.Equals(nameof(Person.AddressId)));
        }

        [Fact]
        public void FindContextIsCorrect()
        {
            // Arrange
            IQueryContext context = null;
            this.personService.OnBeforeExecution += (sender, args) =>
            {
                context = args.Context;
            };

            // Act
            this.personService.Find(p => p.Id == 42);

            // Assert
            context.Should().NotBeNull();
            var selectContext = context as ISelectQueryContext;
            selectContext.QueryTargets.Should().HaveCount(1);
            selectContext.Columns.Should().Satisfy(
                c => c.Name.Equals(nameof(Person.Id)),
                c => c.Name.Equals(nameof(Person.FirstName)),
                c => c.Name.Equals(nameof(Person.LastName)),
                c => c.Name.Equals(nameof(Person.Email)),
                c => c.Name.Equals(nameof(Person.CreatedAt)),
                c => c.Name.Equals(nameof(Person.AddressId)));
            var where = selectContext.WhereClause as IColumn;
            where.EqualityOperator.Should().Be(EqualityOperator.Equals);
            where.Value.Should().Be(42);
        }

        [Fact]
        public void SingleWithEmptyThrows()
        {
            // Arrange
            Action method = () => personService.Single(p => p.Id == 42);
            
            // Assert
            method.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void SingleWithManyThrows()
        {
            // Arrange
            personService.Add(new() { Id = 41 }, new() { Id = 42 });
            Action method = () => personService.Single(p => p.Id == 42);

            // Assert
            method.Should().Throw<InvalidOperationException>();
        }
        
        [Fact]
        public void SingleContextIsCorrect()
        {
            // Arrange
            personService.Add(new Person() { Id = 42 });
            IQueryContext context = null;
            this.personService.OnBeforeExecution += (sender, args) =>
            {
                context = args.Context;
            };

            // Act
            try
            {
                this.personService.Single(p => p.Id == 42);
            }
            catch
            {
                // ignored
            }

            // Assert
            context.Should().NotBeNull();
            var selectContext = context as ISelectQueryContext;
            selectContext.QueryTargets.Should().HaveCount(1);
            selectContext.Columns.Should().Satisfy(
                c => c.Name.Equals(nameof(Person.Id)),
                c => c.Name.Equals(nameof(Person.FirstName)),
                c => c.Name.Equals(nameof(Person.LastName)),
                c => c.Name.Equals(nameof(Person.Email)),
                c => c.Name.Equals(nameof(Person.CreatedAt)),
                c => c.Name.Equals(nameof(Person.AddressId)));
            var where = selectContext.WhereClause as IColumn;
            where.EqualityOperator.Should().Be(EqualityOperator.Equals);
            where.Value.Should().Be(42);
        }
        
        [Fact]
        public void SingleOrDefaultContextIsCorrect()
        {
            // Arrange
            IQueryContext context = null;
            this.personService.OnBeforeExecution += (sender, args) =>
            {
                context = args.Context;
            };

            // Act
            this.personService.SingleOrDefault(p => p.Id == 42);

            // Assert
            context.Should().NotBeNull();
            var selectContext = context as ISelectQueryContext;
            selectContext.QueryTargets.Should().HaveCount(1);
            selectContext.Columns.Should().Satisfy(
                c => c.Name.Equals(nameof(Person.Id)),
                c => c.Name.Equals(nameof(Person.FirstName)),
                c => c.Name.Equals(nameof(Person.LastName)),
                c => c.Name.Equals(nameof(Person.Email)),
                c => c.Name.Equals(nameof(Person.CreatedAt)),
                c => c.Name.Equals(nameof(Person.AddressId)));
            var where = selectContext.WhereClause as IColumn;
            where.EqualityOperator.Should().Be(EqualityOperator.Equals);
            where.Value.Should().Be(42);
        }
        
        [Fact]
        public void FirstContextIsCorrect()
        {
            // Arrange
            IQueryContext context = null;
            this.personService.OnBeforeExecution += (sender, args) =>
            {
                context = args.Context;
            };

            // Act
            try
            {
                this.personService.First(p => p.Id == 42);
            }
            catch
            {
                // ignored
            }

            // Assert
            context.Should().NotBeNull();
            var selectContext = context as ISelectQueryContext;
            selectContext.QueryTargets.Should().HaveCount(1);
            selectContext.Columns.Should().Satisfy(
                c => c.Name.Equals(nameof(Person.Id)),
                c => c.Name.Equals(nameof(Person.FirstName)),
                c => c.Name.Equals(nameof(Person.LastName)),
                c => c.Name.Equals(nameof(Person.Email)),
                c => c.Name.Equals(nameof(Person.CreatedAt)),
                c => c.Name.Equals(nameof(Person.AddressId)));
            var where = selectContext.WhereClause as IColumn;
            where.EqualityOperator.Should().Be(EqualityOperator.Equals);
            where.Value.Should().Be(42);
        }
        
        [Fact]
        public void FirstOrDefaultContextIsCorrect()
        {
            // Arrange
            IQueryContext context = null;
            this.personService.OnBeforeExecution += (sender, args) =>
            {
                context = args.Context;
            };

            // Act
            this.personService.FirstOrDefault(p => p.Id == 42);

            // Assert
            context.Should().NotBeNull();
            var selectContext = context as ISelectQueryContext;
            selectContext.QueryTargets.Should().HaveCount(1);
            selectContext.Columns.Should().Satisfy(
                c => c.Name.Equals(nameof(Person.Id)),
                c => c.Name.Equals(nameof(Person.FirstName)),
                c => c.Name.Equals(nameof(Person.LastName)),
                c => c.Name.Equals(nameof(Person.Email)),
                c => c.Name.Equals(nameof(Person.CreatedAt)),
                c => c.Name.Equals(nameof(Person.AddressId)));
            var where = selectContext.WhereClause as IColumn;
            where.EqualityOperator.Should().Be(EqualityOperator.Equals);
            where.Value.Should().Be(42);
        }

        [Fact]
        public void UpdateContextIsCorrect()
        {
            // Arrange
            IQueryContext context = null;
            this.personService.OnBeforeExecution += (sender, args) =>
            {
                context = args.Context;
            };

            // Act
            this.personService.Update(new
            {
                FirstName = "bob"
            },
            p => p.Id == 42);

            // Assert
            context.Should().NotBeNull();
            var updateContext = context as IUpdateQueryContext;
            updateContext.QueryTargets.Should().HaveCount(1);
            updateContext.Updates.Should().HaveCount(1);
            var where = updateContext.WhereClause as IColumn;
            where.EqualityOperator.Should().Be(EqualityOperator.Equals);
            where.Value.Should().Be(42);
        }
    }
}

