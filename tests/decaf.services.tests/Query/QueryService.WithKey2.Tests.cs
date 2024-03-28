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
    public class QueryServiceWithKey2Tests
    {
        private readonly IService<Address, int, int> addressService;

        public QueryServiceWithKey2Tests()
        {
            var services = new ServiceCollection();
            services.AddDecaf(o =>
            {
                o.TrackUnitsOfWork();
                o.OverrideDefaultLogLevel(LogLevel.Debug);
                o.UseMockDatabase().WithMockConnectionDetails();
            });
            services.AddDecafService<Address, int, int>().AsScoped();

            var provider = services.BuildServiceProvider();
            this.addressService = provider.GetService<IService<Address, int, int>>();
        }

        [Fact]
        public void GetAllDoesNotThrow()
        {
            // Arrange

            // Act
            Action method = () =>
            {
                this.addressService.All();
            };

            // Assert
            method.Should().NotThrow();
        }

        [Fact]
        public void GetAllContextIsCorrect()
        {
            // Arrange
            IQueryContext context = null;
            this.addressService.OnBeforeExecution += (sender, args) =>
            {
                context = args.Context;
            };

            // Act
            this.addressService.All();

            // Assert
            context.Should().NotBeNull();
            var selectContext = context as ISelectQueryContext;
            selectContext.QueryTargets.Should().HaveCount(1);
            selectContext.Columns.Should().Satisfy(
                c => c.Name.Equals(nameof(Address.Id)),
                c => c.Name.Equals(nameof(Address.PersonId)),
                c => c.Name.Equals(nameof(Address.Line1)),
                c => c.Name.Equals(nameof(Address.Line2)),
                c => c.Name.Equals(nameof(Address.Line3)),
                c => c.Name.Equals(nameof(Address.Line4)),
                c => c.Name.Equals(nameof(Address.City)),
                c => c.Name.Equals(nameof(Address.PostCode)),
                c => c.Name.Equals(nameof(Address.Region)),
                c => c.Name.Equals(nameof(Address.Country)));
        }

        [Fact]
        public void GetContextIsCorrect()
        {
            // Arrange
            IQueryContext context = null;
            this.addressService.OnBeforeExecution += (sender, args) =>
            {
                context = args.Context;
            };

            // Act
            this.addressService.Find(p => p.Id == 42);

            // Assert
            context.Should().NotBeNull();
            var selectContext = context as ISelectQueryContext;
            selectContext.QueryTargets.Should().HaveCount(1);
            selectContext.Columns.Should().Satisfy(
                c => c.Name.Equals(nameof(Address.Id)),
                c => c.Name.Equals(nameof(Address.PersonId)),
                c => c.Name.Equals(nameof(Address.Line1)),
                c => c.Name.Equals(nameof(Address.Line2)),
                c => c.Name.Equals(nameof(Address.Line3)),
                c => c.Name.Equals(nameof(Address.Line4)),
                c => c.Name.Equals(nameof(Address.City)),
                c => c.Name.Equals(nameof(Address.PostCode)),
                c => c.Name.Equals(nameof(Address.Region)),
                c => c.Name.Equals(nameof(Address.Country)));
            var where = selectContext.WhereClause as IColumn;
            where.EqualityOperator.Should().Be(EqualityOperator.Equals);
            where.Value.Should().Be(42);
        }

        [Fact]
        public void GetByIdContextIsCorrect()
        {
            // Arrange
            IQueryContext context = null;
            this.addressService.OnBeforeExecution += (sender, args) =>
            {
                context = args.Context;
            };

            // Act
            this.addressService.Get(1, 42);

            // Assert
            context.Should().NotBeNull();
            var selectContext = context as ISelectQueryContext;
            selectContext.QueryTargets.Should().HaveCount(1);
            selectContext.Columns.Should().Satisfy(
                c => c.Name.Equals(nameof(Address.Id)),
                c => c.Name.Equals(nameof(Address.PersonId)),
                c => c.Name.Equals(nameof(Address.Line1)),
                c => c.Name.Equals(nameof(Address.Line2)),
                c => c.Name.Equals(nameof(Address.Line3)),
                c => c.Name.Equals(nameof(Address.Line4)),
                c => c.Name.Equals(nameof(Address.City)),
                c => c.Name.Equals(nameof(Address.PostCode)),
                c => c.Name.Equals(nameof(Address.Region)),
                c => c.Name.Equals(nameof(Address.Country)));
            var where = selectContext.WhereClause as state.Conditionals.And;
            where.Children.Should().HaveCount(2);
        }

        [Fact]
        public void GetByIdArrayContextIsCorrect()
        {
            // Arrange
            IQueryContext context = null;
            this.addressService.OnBeforeExecution += (sender, args) =>
            {
                context = args.Context;
            };
            var key = CompositeKeyValue.Create(42, 43);

            // Act
            this.addressService.Get(new[] { CompositeKeyValue.Create(42, 43) });

            // Assert
            context.Should().NotBeNull();
            var selectContext = context as ISelectQueryContext;
            selectContext.QueryTargets.Should().HaveCount(1);
            selectContext.Columns.Should().Satisfy(
                c => c.Name.Equals(nameof(Address.Id)),
                c => c.Name.Equals(nameof(Address.City)),
                c => c.Name.Equals(nameof(Address.Country)),
                c => c.Name.Equals(nameof(Address.Line1)),
                c => c.Name.Equals(nameof(Address.Line2)),
                c => c.Name.Equals(nameof(Address.Line3)),
                c => c.Name.Equals(nameof(Address.Line4)),
                c => c.Name.Equals(nameof(Address.PersonId)),
                c => c.Name.Equals(nameof(Address.PostCode)),
                c => c.Name.Equals(nameof(Address.Region)));
            var where = (And)selectContext.WhereClause;
            where.Should().NotBeNull();
            where.Children.Should().Satisfy(
                c => ((Column<int>)c).Value == 42,
                c => ((Column<int>)c).Value == 43);
        }

        [Fact]
        public void GetByIdEnumerableContextIsCorrect()
        {
            // Arrange
            IQueryContext context = null;
            this.addressService.OnBeforeExecution += (sender, args) =>
            {
                context = args.Context;
            };
            var key = CompositeKeyValue.Create(42, 43);

            // Act
            this.addressService.Get(new List<ICompositeKeyValue<int, int>> { key });

            // Assert
            context.Should().NotBeNull();
            var selectContext = context as ISelectQueryContext;
            selectContext.QueryTargets.Should().HaveCount(1);
            selectContext.Columns.Should().Satisfy(
                c => c.Name.Equals(nameof(Address.Id)),
                c => c.Name.Equals(nameof(Address.City)),
                c => c.Name.Equals(nameof(Address.Country)),
                c => c.Name.Equals(nameof(Address.Line1)),
                c => c.Name.Equals(nameof(Address.Line2)),
                c => c.Name.Equals(nameof(Address.Line3)),
                c => c.Name.Equals(nameof(Address.Line4)),
                c => c.Name.Equals(nameof(Address.PersonId)),
                c => c.Name.Equals(nameof(Address.PostCode)),
                c => c.Name.Equals(nameof(Address.Region)));
            var where = (And)selectContext.WhereClause;
            where.Should().NotBeNull();
            where.Children.Should().Satisfy(
                c => ((Column<int>)c).Value == 42,
                c => ((Column<int>)c).Value == 43);
        }
    }
}

