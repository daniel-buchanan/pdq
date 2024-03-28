using System;
using System.Collections.Generic;
using decaf.common;
using decaf.state;
using decaf.state.Conditionals;
using decaf.tests.common.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using decaf.tests.common.Mocks;
using decaf.services;
using Xunit;

namespace decaf.services.tests.Query
{
    public class QueryServiceWithKey1Tests
    {
        private readonly IService<Person, int> personService;

        public QueryServiceWithKey1Tests()
        {
            var services = new ServiceCollection();
            services.AddDecaf(o =>
            {
                o.TrackUnitsOfWork();
                o.OverrideDefaultLogLevel(LogLevel.Debug);
                o.UseMockDatabase().WithMockConnectionDetails();
            });
            services.AddDecafService<Person, int>().AsScoped();

            var provider = services.BuildServiceProvider();
            this.personService = provider.GetService<IService<Person, int>>();
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
        public void GetAllContextIsCorrect()
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
        public void GetContextIsCorrect()
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
        public void GetByIdContextIsCorrect()
        {
            // Arrange
            IQueryContext context = null;
            this.personService.OnBeforeExecution += (sender, args) =>
            {
                context = args.Context;
            };

            // Act
            this.personService.Get(42);

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
            var where = (IInValues)selectContext.WhereClause;
            where.Should().NotBeNull();
            where.GetValues().Should().Contain(42);
        }

        [Fact]
        public void GetByIdArrayContextIsCorrect()
        {
            // Arrange
            IQueryContext context = null;
            this.personService.OnBeforeExecution += (sender, args) =>
            {
                context = args.Context;
            };

            // Act
            this.personService.Get(new[] { 42, 43 });

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
            var where = (IInValues)selectContext.WhereClause;
            where.Should().NotBeNull();
            where.GetValues().Should().Satisfy(
                c => ((int)c) == 42,
                c => ((int)c) == 43);
        }

        [Fact]
        public void GetByIdEnumerableContextIsCorrect()
        {
            // Arrange
            IQueryContext context = null;
            this.personService.OnBeforeExecution += (sender, args) =>
            {
                context = args.Context;
            };

            // Act
            this.personService.Get(new List<int> { 42, 43 });

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
            var where = (IInValues)selectContext.WhereClause;
            where.Should().NotBeNull();
            where.GetValues().Should().Satisfy(
                c => ((int)c) == 42,
                c => ((int)c) == 43);
        }
    }
}