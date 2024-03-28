using System;
using System.Linq;
using decaf.common;
using decaf.state;
using decaf.state.Conditionals;
using decaf.state.ValueSources.Update;
using decaf.tests.common.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using decaf.tests.common.Mocks;
using Xunit;

namespace decaf.core_tests
{
    public class UpdateTests
    {
        private IQueryContainerInternal query;

        public UpdateTests()
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
        public void SimpleTypedUpdateSucceeds()
        {
            // Act
            this.query
                .Update()
                .Table<Person>()
                .Set(p => p.Email, "bob@bob.com")
                .Where(p => p.Id == 42);

            // Assert
            var context = this.query.Context as IUpdateQueryContext;
            context.QueryTargets.Should().HaveCount(1);
            context.Updates.Should().HaveCount(1);
            var upd = context.Updates.First() as StaticValueSource;
            var updatedValue = upd.GetValue<string>();
            updatedValue.Should().Be("bob@bob.com");
            var where = context.WhereClause as IColumn;
            where.Should().NotBeNull();
            where.Details.Name.Should().Be(nameof(Person.Id));
            where.Details.Source.Alias.Should().Be("p0");
            where.Value.Should().BeEquivalentTo(42);
        }

        [Fact]
        public void SimpleTypedUpdateWithAliasSucceeds()
        {
            // Act
            this.query
                .Update()
                .Table<Person>(p => p)
                .Set(p => p.Email, "bob@bob.com")
                .Where(p => p.Id == 42)
                .Output(p => p.Id);

            // Assert
            var context = this.query.Context as IUpdateQueryContext;
            context.QueryTargets.Should().HaveCount(1);
            context.Updates.Should().HaveCount(1);
            var where = context.WhereClause as IColumn;
            where.Should().NotBeNull();
            where.Details.Name.Should().Be(nameof(Person.Id));
            where.Details.Source.Alias.Should().Be("p");
            where.Value.Should().BeEquivalentTo(42);
        }

        [Fact]
        public void SimpleTypedUpdateWithConcreteSetSucceeds()
        {
            // Act
            this.query
                .Update()
                .Table<Person>(p => p)
                .Set(new Person
                {
                    LastName = "Smith"
                })
                .Where(p => p.Id == 42);

            // Assert
            var context = this.query.Context as IUpdateQueryContext;
            context.QueryTargets.Should().HaveCount(1);
            context.Updates.Should().HaveCount(1);
            var where = context.WhereClause as IColumn;
            where.Should().NotBeNull();
            where.Details.Name.Should().Be(nameof(Person.Id));
            where.Details.Source.Alias.Should().Be("p");
            where.Value.Should().BeEquivalentTo(42);
        }

        [Fact]
        public void SimpleUpdateSucceeds()
        {
            // Act
            this.query
                .Update()
                .Table("users", "u")
                .Set(new
                {
                    last_name = "Smith"
                })
                .Where(b => b.Column("Id", "u").Is().EqualTo(42));

            // Assert
            var context = this.query.Context as IUpdateQueryContext;
            context.QueryTargets.Should().HaveCount(1);
            context.Updates.Should().HaveCount(1);
            var and = context.WhereClause as And;
            and.Should().NotBeNull();
            var where = and.Children.First() as IColumn;
            where.Should().NotBeNull();
            where.Details.Name.Should().Be("Id");
            where.Details.Source.Alias.Should().Be("u");
            where.Value.Should().BeEquivalentTo(42);
        }

        [Fact]
        public void SimpleUpdateWithTypedSetSucceeds()
        {
            // Act
            this.query
                .Update()
                .Table("users", "u")
                .Set("created_at", DateTime.Now)
                .Where(b => b.Column("Id", "u").Is().EqualTo(42));

            // Assert
            var context = this.query.Context as IUpdateQueryContext;
            context.QueryTargets.Should().HaveCount(1);
            context.Updates.Should().HaveCount(1);
            var and = context.WhereClause as And;
            and.Should().NotBeNull();
            var where = and.Children.First() as IColumn;
            where.Should().NotBeNull();
            where.Details.Name.Should().Be("Id");
            where.Details.Source.Alias.Should().Be("u");
            where.Value.Should().BeEquivalentTo(42);
        }

        [Fact]
        public void SimpleUpdateFromQuerySucceeds()
        {
            // Act
            this.query
                .Update()
                .Table("users", "u")
                .From(b =>
                {
                    b.From("person", "p")
                        .Where(w => w.Column("id", "p").Is().EqualTo().Column("id", "u"))
                        .Select(s => new
                        {
                            first_name = s.Is<string>("firstName", "p")
                        });
                })
                .Set("first_name", "first_name")
                .Where(b => b.Column("Id", "u").Is().EqualTo(42))
                .Output("id");

            // Assert
            var context = this.query.Context as IUpdateQueryContext;
            context.QueryTargets.Should().HaveCount(1);
            context.Updates.Should().HaveCount(1);
            var and = context.WhereClause as And;
            and.Should().NotBeNull();
            var where = and.Children.First() as IColumn;
            where.Should().NotBeNull();
            where.Details.Name.Should().Be("Id");
            where.Details.Source.Alias.Should().Be("u");
            where.Value.Should().BeEquivalentTo(42);
        }

        [Fact]
        public void SimpleTypedUpdateFromQUerySucceeds()
        {
            // Act
            this.query
                .Update()
                .Table<User>(u => u)
                .From<Person>(b =>
                {
                    b.From<Person>(p => p)
                        .Where(p => p.LastName == null)
                        .Select(p => new Person
                        {
                            Id = p.Id,
                            FirstName = p.FirstName
                        });
                })
                .Set(u => u.FirstName, p => p.FirstName)
                .Where((u, p) => u.Id == 42)
                .Output(u => u.Id);

            // Assert
            var context = this.query.Context as IUpdateQueryContext;
            context.QueryTargets.Should().HaveCount(1);
            context.Updates.Should().HaveCount(1);
            var where = context.WhereClause as IColumn;
            where.Should().NotBeNull();
            where.Details.Name.Should().Be("Id");
            where.Details.Source.Alias.Should().Be("u");
            where.Value.Should().BeEquivalentTo(42);
        }
    }
}

