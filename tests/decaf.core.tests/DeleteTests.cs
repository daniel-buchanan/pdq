using decaf.common;
using decaf.common.Connections;
using decaf.common.ValueFunctions;
using decaf.state;
using decaf.state.Conditionals;
using decaf.tests.common.Mocks;
using decaf.tests.common.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace decaf.core_tests
{
    public class DeleteTests
    {
        private IQueryContainerInternal query;

        public DeleteTests()
        {
            var services = new ServiceCollection();
            services.AddDecaf(o =>
            {
                o.TrackUnitsOfWork();
                o.OverrideDefaultLogLevel(LogLevel.Debug);
                o.UseMockDatabase();
            });
            services.AddScoped<IConnectionDetails, MockConnectionDetails>();

            var provider = services.BuildServiceProvider();
            var decaf = provider.GetService<IDecaf>();
            var transient = decaf.BuildUnit();
            this.query = transient.Query() as IQueryContainerInternal;
        }

        [Fact]
        public void SimpleDeleteSucceeds()
        {
            // Act
            this.query
                .Delete()
                .From<Person>(p => p)
                .Where(p => p.LastName.Contains("smith"))
                .Output(p => p.Id);

            // Assert
            var context = this.query.Context as IDeleteQueryContext;
            context.QueryTargets.Should().HaveCount(1);
            var where = context.WhereClause as IColumn;
            where.Should().NotBeNull();
            where.Details.Name.Should().Be(nameof(Person.LastName));
            where.Details.Source.Alias.Should().Be("p");
            where.ValueFunction.Should().BeAssignableTo(typeof(StringContains));
        }

        [Fact]
        public void SimpleDeleteWithoutAliasSucceeds()
        {
            // Act
            this.query
                .Delete()
                .From<Person>()
                .Where(p => p.LastName.Contains("smith"));

            // Assert
            var context = this.query.Context as IDeleteQueryContext;
            context.QueryTargets.Should().HaveCount(1);
            var where = context.WhereClause as IColumn;
            where.Should().NotBeNull();
            where.Details.Name.Should().Be(nameof(Person.LastName));
            where.Details.Source.Alias.Should().Be("p0");
            where.ValueFunction.Should().BeAssignableTo(typeof(StringContains));
        }

        [Fact]
        public void SimpleDeleteUnTypedSucceeds()
        {
            // Act
            this.query
                .Delete()
                .From("person", "p")
                .Where(b => b.Column("last_name", "p").Is().Like("smith"))
                .Output("id");

            // Assert
            var context = this.query.Context as IDeleteQueryContext;
            context.QueryTargets.Should().HaveCount(1);
            var where = context.WhereClause as state.Conditionals.IColumn;
            where.Should().NotBeNull();
            where.Details.Name.Should().Be("last_name");
            where.Details.Source.Alias.Should().Be("p");
            where.ValueFunction.Should().BeAssignableTo(typeof(StringContains));
        }
    }
}

