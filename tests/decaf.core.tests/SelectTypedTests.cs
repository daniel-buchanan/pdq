using decaf.common;
using decaf.common.ValueFunctions;
using decaf.state;
using decaf.state.Conditionals;
using decaf.tests.common.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using decaf.tests.common.Mocks;
using Xunit;
using DatePart = decaf.common.DatePart;

namespace decaf.core_tests
{
    public class SelectTypedTests
    {
        private IQueryContainerInternal query;

        public SelectTypedTests()
        {
            var services = new ServiceCollection();
            services.AddDecaf(o =>
            {
                o.TrackUnitsOfWork();
                o.OverrideDefaultLogLevel(LogLevel.Debug);
                o.UseMockDatabase().WithMockConnectionDetails();
            });

            var provider = services.BuildServiceProvider();
            var decaf = provider.GetService<IDecaf>();
            var transient = decaf.BuildUnit();
            this.query = transient.Query() as IQueryContainerInternal;
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

        [Fact]
        public void SelectAllSucceeds()
        {
            // Act
            this.query
                .Select()
                .From<Person>(p => p)
                .Where(p => p.Id == 42)
                .SelectAll<Person>(p => p);

            // Assert
            var context = this.query.Context as ISelectQueryContext;
            context.QueryTargets.Should().HaveCount(1);
            context.Columns.Should().Satisfy(
                c => c.Name.Equals(nameof(Person.Id)),
                c => c.Name.Equals(nameof(Person.FirstName)),
                c => c.Name.Equals(nameof(Person.LastName)),
                c => c.Name.Equals(nameof(Person.Email)),
                c => c.Name.Equals(nameof(Person.CreatedAt)),
                c => c.Name.Equals(nameof(Person.AddressId)));
        }

        [Fact]
        public void SelectWithTwoTablesSucceeds()
        {
            // Act
            this.query
                .Select()
                .From<Person>(p => p)
                .From<Address>(a => a)
                .Where((p, a) => p.LastName.Contains("smith"));

            // Assert
            var context = this.query.Context as ISelectQueryContext;
            context.QueryTargets.Should().HaveCount(2);
            var where = context.WhereClause as IColumn;
            where.Should().NotBeNull();
            where.Details.Name.Should().Be(nameof(Person.LastName));
            where.Details.Source.Alias.Should().Be("p");
            where.ValueFunction.Should().BeAssignableTo(typeof(StringContains));
        }

        [Fact]
        public void SelectWithTwoTablesNoAliasSucceeds()
        {
            // Act
            this.query
                .Select()
                .From<Person>()
                .From<Address>()
                .Where((p, a) => p.LastName.Contains("smith"));

            // Assert
            var context = this.query.Context as ISelectQueryContext;
            context.QueryTargets.Should().HaveCount(2);
            var where = context.WhereClause as IColumn;
            where.Should().NotBeNull();
            where.Details.Name.Should().Be(nameof(Person.LastName));
            where.Details.Source.Alias.Should().Be("p0");
            where.ValueFunction.Should().BeAssignableTo(typeof(StringContains));
        }

        [Fact]
        public void SelectWithThreeTablesSucceeds()
        {
            // Act
            this.query
                .Select()
                .From<Person>(p => p)
                .From<Address>(a => a)
                .From<User>(u => u)
                .Where((p, a, u) => p.LastName.Contains("smith"));

            // Assert
            var context = this.query.Context as ISelectQueryContext;
            context.QueryTargets.Should().HaveCount(3);
            var where = context.WhereClause as IColumn;
            where.Should().NotBeNull();
            where.Details.Name.Should().Be(nameof(Person.LastName));
            where.Details.Source.Alias.Should().Be("p");
            where.ValueFunction.Should().BeAssignableTo(typeof(StringContains));
        }

        [Fact]
        public void SelectWithThreeTablesNoAliasSucceeds()
        {
            // Act
            this.query
                .Select()
                .From<Person>()
                .From<Address>()
                .From<User>()
                .Where((p, a, u) => p.LastName.Contains("smith"));

            // Assert
            var context = this.query.Context as ISelectQueryContext;
            context.QueryTargets.Should().HaveCount(3);
            var where = context.WhereClause as IColumn;
            where.Should().NotBeNull();
            where.Details.Name.Should().Be(nameof(Person.LastName));
            where.Details.Source.Alias.Should().Be("p0");
            where.ValueFunction.Should().BeAssignableTo(typeof(StringContains));
        }

        [Fact]
        public void SelectWithFourTablesSucceeds()
        {
            // Act
            this.query
                .Select()
                .From<Person>(p => p)
                .From<Address>(a => a)
                .From<User>(u => u)
                .From<Note>(n => n)
                .Where((p, a, u, n) => p.LastName.Contains("smith"));

            // Assert
            var context = this.query.Context as ISelectQueryContext;
            context.QueryTargets.Should().HaveCount(4);
            var where = context.WhereClause as IColumn;
            where.Should().NotBeNull();
            where.Details.Name.Should().Be(nameof(Person.LastName));
            where.Details.Source.Alias.Should().Be("p");
            where.ValueFunction.Should().BeAssignableTo(typeof(StringContains));
        }

        [Fact]
        public void SelectWithFourTablesNoAliasSucceeds()
        {
            // Act
            this.query
                .Select()
                .From<Person>()
                .From<Address>()
                .From<User>()
                .From<Note>()
                .Where((p, a, u, n) => p.LastName.Contains("smith"));

            // Assert
            var context = this.query.Context as ISelectQueryContext;
            context.QueryTargets.Should().HaveCount(4);
            var where = context.WhereClause as IColumn;
            where.Should().NotBeNull();
            where.Details.Name.Should().Be(nameof(Person.LastName));
            where.Details.Source.Alias.Should().Be("p0");
            where.ValueFunction.Should().BeAssignableTo(typeof(StringContains));
        }

        [Fact]
        public void SelectWithFiveTablesSucceeds()
        {
            // Act
            this.query
                .Select()
                .From<Person>(p => p)
                .From<Address>(a => a)
                .From<User>(u => u)
                .From<Note>(n => n)
                .From<AddressNote>(an => an)
                .Where((p, a, u, n, an) => p.LastName.Contains("smith"));

            // Assert
            var context = this.query.Context as ISelectQueryContext;
            context.QueryTargets.Should().HaveCount(5);
            var where = context.WhereClause as IColumn;
            where.Should().NotBeNull();
            where.Details.Name.Should().Be(nameof(Person.LastName));
            where.Details.Source.Alias.Should().Be("p");
            where.ValueFunction.Should().BeAssignableTo(typeof(StringContains));
        }

        [Fact]
        public void SelectWithFiveTablesNoAliasSucceeds()
        {
            // Act
            this.query
                .Select()
                .From<Person>()
                .From<Address>()
                .From<User>()
                .From<Note>()
                .From<AddressNote>()
                .Where((p, a, u, n, an) => p.LastName.Contains("smith"));

            // Assert
            var context = this.query.Context as ISelectQueryContext;
            context.QueryTargets.Should().HaveCount(5);
            var where = context.WhereClause as IColumn;
            where.Should().NotBeNull();
            where.Details.Name.Should().Be(nameof(Person.LastName));
            where.Details.Source.Alias.Should().Be("p0");
            where.ValueFunction.Should().BeAssignableTo(typeof(StringContains));
        }
    }
}

