using System;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using pdq.common;
using pdq.core_tests.Mocks;
using pdq.core_tests.Models;
using pdq.state;
using Xunit;

namespace pdq.core_tests
{
    public class SelectTypedTests
    {
        private IQueryInternal query;

        public SelectTypedTests()
        {
            var services = new ServiceCollection();
            services.AddPdq(o =>
            {
                o.EnableTransientTracking();
                o.OverrideDefaultLogLevel(LogLevel.Debug);
                o.UseMockDatabase();
            });
            services.AddScoped<IConnectionDetails, MockConnectionDetails>();

            var provider = services.BuildServiceProvider();
            var uow = provider.GetService<IUnitOfWork>();
            var transient = uow.Begin();
            this.query = transient.Query() as IQueryInternal;
        }

        [Fact]
        public void SimpleSelectSucceeds()
        {
            // Act
            this.query
                .Select()
                .From<Person>(p => p)
                .Join<Address>((p, a) => p.AddressId == a.Id)
                .Where((p, a) => p.LastName.Contains("smith"));

            // Assert
            var context = this.query.Context as ISelectQueryContext;
            context.QueryTargets.Should().HaveCount(2);
            context.Joins.Should().HaveCount(1);
            context.WhereClause.Should().BeAssignableTo(typeof(state.Conditionals.IColumn));
        }

        [Fact]
        public void SimpleSelect2Succeeds()
        {
            // Act
            this.query
                .Select()
                .From<Person>(p => p)
                .Join<Address>((p, a) => p.AddressId == a.Id)
                .Where((p, a) => p.LastName.Contains("smith") && a.PostCode == "3216");

            // Assert
            var context = this.query.Context as ISelectQueryContext;
            context.QueryTargets.Should().HaveCount(2);
            context.Joins.Should().HaveCount(1);
            context.WhereClause.Should().BeAssignableTo(typeof(state.Conditionals.And));
        }

        [Fact]
        public void SimpleSelectDateTimeSucceeds()
        {
            // Act
            this.query
                .Select()
                .From<Person>(p => p)
                .Join<Address>((p, a) => p.AddressId == a.Id)
                .Where((p, a) => p.LastName.Contains("smith") && a.PostCode == "3216")
                .Select((p, a) => new Result
                {
                    City = a.City,
                    Name = p.FirstName,
                    Timestamp = p.CreatedAt.DatePart(DatePart.Year)
                });

            // Assert
            var context = this.query.Context as ISelectQueryContext;
            context.QueryTargets.Should().HaveCount(2);
            context.Joins.Should().HaveCount(1);
            context.WhereClause.Should().BeAssignableTo(typeof(state.Conditionals.And));
        }
    }
}

