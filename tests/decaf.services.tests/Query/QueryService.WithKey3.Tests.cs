using System;
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
    public class QueryServiceWithKey3Tests
    {
        private readonly IService<AddressNote, int, int, int> addressService;

        public QueryServiceWithKey3Tests()
        {
            var services = new ServiceCollection();
            services.AddDecaf(o =>
            {
                o.TrackUnitsOfWork();
                o.OverrideDefaultLogLevel(LogLevel.Debug);
                o.UseMockDatabase().WithMockConnectionDetails();
            });
            services.AddDecafService<AddressNote, int, int, int>().AsScoped();

            var provider = services.BuildServiceProvider();
            this.addressService = provider.GetService<IService<AddressNote, int, int, int>>();
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
                c => c.Name.Equals(nameof(AddressNote.Id)),
                c => c.Name.Equals(nameof(AddressNote.AddressId)),
                c => c.Name.Equals(nameof(AddressNote.PersonId)),
                c => c.Name.Equals(nameof(AddressNote.Value)));
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
                c => c.Name.Equals(nameof(AddressNote.Id)),
                c => c.Name.Equals(nameof(AddressNote.AddressId)),
                c => c.Name.Equals(nameof(AddressNote.PersonId)),
                c => c.Name.Equals(nameof(AddressNote.Value)));
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
            this.addressService.Get(1, 42, 43);

            // Assert
            context.Should().NotBeNull();
            var selectContext = context as ISelectQueryContext;
            selectContext.QueryTargets.Should().HaveCount(1);
            selectContext.Columns.Should().Satisfy(
                c => c.Name.Equals(nameof(AddressNote.Id)),
                c => c.Name.Equals(nameof(AddressNote.AddressId)),
                c => c.Name.Equals(nameof(AddressNote.PersonId)),
                c => c.Name.Equals(nameof(AddressNote.Value)));
            var where = selectContext.WhereClause as state.Conditionals.And;
            where.Children.Should().HaveCount(3);
        }
    }
}

