using System;
using decaf.common;
using decaf.common.Connections;
using decaf.state;
using decaf.tests.common.Mocks;
using decaf.tests.common.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace decaf.core_tests
{
    public class InsertTests
    {
        private IQueryContainerInternal query;

        public InsertTests()
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
        public void SimpleInsertSucceeds()
        {
            // Act
            this.query.Insert()
                .Into("users")
                .Columns(b => new
                {
                    email = b.Is<string>(),
                    first_name = b.Is<string>(),
                    last_name = b.Is<string>()
                })
                .Value(new
                {
                    email = "bob@bob.com",
                    first_name = "Bob",
                    last_name = "Smith"
                })
                .Output("id");

            // Assert
            var context = this.query.Context as IInsertQueryContext;
            context.Should().NotBeNull();
            context.Target.Name.Should().Be("users");
            context.Columns.Count.Should().Be(3);
            context.Source.Should().BeAssignableTo<IInsertStaticValuesSource>();
        }

        [Fact]
        public void SimpleInsertMultipleValuesSucceeds()
        {
            // Arrange
            var objects = new dynamic[]
            {
                new
                {
                    email = "bob@bob.com",
                    first_name = "Bob",
                    last_name = "Smith"
                },
                new
                {
                    email = "jane@bob.com",
                    first_name = "Jane",
                    last_name = "Doe"
                }
            };

            // Act
            this.query.Insert()
                .Into("users")
                .Columns(b => new
                {
                    email = b.Is<string>(),
                    first_name = b.Is<string>(),
                    last_name = b.Is<string>()
                })
                .Values(objects);

            // Assert
            var context = this.query.Context as IInsertQueryContext;
            context.Should().NotBeNull();
            context.Target.Name.Should().Be("users");
            context.Columns.Count.Should().Be(3);
            var source = context.Source as IInsertStaticValuesSource;
            source.Should().NotBeNull();
            source.Values.Should().HaveCount(2);
        }

        [Fact]
        public void InsertFromQuerySucceeds()
        {
            // Arrange
            Action<ISelect> fromQuery = (b) =>
            {
                var fromDate = DateTime.Parse("2022-01-01");
                b.From<Person>(p => p)
                    .Where(p => p.CreatedAt > fromDate)
                    .Select(p => new
                    {
                        email = p.Email,
                        first_name = p.FirstName,
                        last_name = p.LastName
                    });
            };

            // Act
            this.query.Insert()
                .Into("users", "b")
                .Columns(b => new
                {
                    email = b.Is<string>(),
                    first_name = b.Is<string>(),
                    last_name = b.Is<string>()
                })
                .From(fromQuery);

            // Assert
            var context = this.query.Context as IInsertQueryContext;
            context.Should().NotBeNull();
            context.Target.Name.Should().Be("users");
            context.Columns.Count.Should().Be(3);
            var source = context.Source as IInsertQueryValuesSource;
            source.Should().NotBeNull();
            source.Query.Should().NotBeNull();
        }

        [Fact]
        public void SimpleTypedInsertSucceeds()
        {
            // Act
            this.query.Insert()
                .Into<Person>()
                .Columns(p => new
                {
                    p.AddressId,
                    p.Email,
                    p.FirstName,
                    p.LastName,
                    p.CreatedAt
                })
                .Value(new Person
                {
                    AddressId = 42,
                    Email = "bob@bob.com",
                    FirstName = "Bob",
                    LastName = "Smith",
                    CreatedAt = DateTime.UtcNow
                })
                .Output(p => p.Id);

            // Assert
            var context = this.query.Context as IInsertQueryContext;
            context.Should().NotBeNull();
            context.Target.Name.Should().Be(nameof(Person));
            context.Columns.Count.Should().Be(5);
            var source = context.Source as IInsertStaticValuesSource;
            source.Should().NotBeNull();
            source.Values.Should().HaveCount(1);
        }

        [Fact]
        public void SimpleTypedInsertMultipleValuesSucceeds()
        {
            // Arrange
            var objects = new Person[]
            {
                new Person
                {
                    Email = "bob@bob.com",
                    FirstName = "Bob",
                    LastName = "Smith",
                    CreatedAt = DateTime.Now,
                    AddressId = 42
                },
                new Person
                {
                    Email = "jane@bob.com",
                    FirstName = "Jane",
                    LastName = "Doe",
                    CreatedAt = DateTime.Now,
                    AddressId = 43
                }
            };

            // Act
            this.query.Insert()
                .Into<Person>()
                .Columns(p => new
                {
                    p.AddressId,
                    p.Email,
                    p.FirstName,
                    p.LastName,
                    p.CreatedAt
                })
                .Values(objects);

            // Assert
            var context = this.query.Context as IInsertQueryContext;
            context.Should().NotBeNull();
            context.Target.Name.Should().Be(nameof(Person));
            context.Columns.Count.Should().Be(5);
            var source = context.Source as IInsertStaticValuesSource;
            source.Should().NotBeNull();
            source.Values.Should().HaveCount(2);
        }

        [Fact]
        public void InsertTypedFromQuerySucceeds()
        {
            // Arrange
            Action<ISelect> fromQuery = (b) =>
            {
                var fromDate = DateTime.Parse("2022-01-01");
                b.From("users", "u")
                    .Where(bb => bb.Column("created_at", "u").Is().GreaterThan(fromDate))
                    .Select(bb => new
                    {
                        Email = bb.Is<string>("email", "u"),
                        FirstName = bb.Is<string>("first_name", "u"),
                        LastName = bb.Is<string>("last_name", "u")
                    }); ;
            };

            // Act
            this.query.Insert()
                .Into<Person>()
                .Columns(p => new
                {
                    p.Email,
                    p.FirstName,
                    p.LastName
                })
                .From(fromQuery);

            // Assert
            var context = this.query.Context as IInsertQueryContext;
            context.Should().NotBeNull();
            context.Target.Name.Should().Be(nameof(Person));
            context.Columns.Count.Should().Be(3);
            var source = context.Source as IInsertQueryValuesSource;
            source.Should().NotBeNull();
            source.Query.Should().NotBeNull();
        }
    }
}

