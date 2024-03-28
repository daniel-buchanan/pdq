using System;
using System.Collections.Generic;
using System.Linq;
using decaf.common;
using decaf.state;
using decaf.tests.common.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using decaf.tests.common.Mocks;
using decaf.services;
using Xunit;

namespace decaf.services.tests.Command
{
    public class CommandServiceWithKey1Tests
    {
        private readonly IService<Person, int> personService;

        public CommandServiceWithKey1Tests()
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
        public void AddSucceeds()
        {
            // Arrange
            IQueryContext context = null;
            this.personService.OnBeforeExecution += (sender, args) =>
            {
                context = args.Context;
            };

            // Act
            this.personService.Add(new Person
            {
                Email = "bob@bob.com"
            });

            // Assert
            context.Should().NotBeNull();
            var insertContext = context as IInsertQueryContext;
            insertContext.Source.Should().BeOfType<state.ValueSources.Insert.StaticValuesSource>();
        }

        [Fact]
        public void AddSetSucceeds()
        {
            // Arrange
            IQueryContext context = null;
            this.personService.OnBeforeExecution += (sender, args) =>
            {
                context = args.Context;
            };

            // Act
            this.personService.Add(new Person
            {
                Email = "bob@bob.com"
            },
            new Person
            {
                Email = "james@bob.com"
            });

            // Assert
            context.Should().NotBeNull();
            var insertContext = context as IInsertQueryContext;
            insertContext.Source.Should().BeOfType<state.ValueSources.Insert.StaticValuesSource>();
            var values = insertContext.Source as IInsertStaticValuesSource;
            values.Values.Should().HaveCount(2);
        }

        [Fact]
        public void AddEnumerableSetSucceeds()
        {
            // Arrange
            IQueryContext context = null;
            this.personService.OnBeforeExecution += (sender, args) =>
            {
                context = args.Context;
            };

            // Act
            this.personService.Add(new List<Person> {
                new Person
                {
                    Email = "bob@bob.com"
                },
                new Person
                {
                    Email = "james@bob.com"
                }
            });

            // Assert
            context.Should().NotBeNull();
            var insertContext = context as IInsertQueryContext;
            insertContext.Source.Should().BeOfType<state.ValueSources.Insert.StaticValuesSource>();
            var values = insertContext.Source as IInsertStaticValuesSource;
            values.Values.Should().HaveCount(2);
        }

        [Fact]
        public void AddEnumerableWithEmptyReturnsEmptyEnumeration()
        {
            // Arrange
            IQueryContext context = null;
            this.personService.OnBeforeExecution += (sender, args) =>
            {
                context = args.Context;
            };
            var items = Enumerable.Empty<Person>();

            // Act
            var results = this.personService.Add(items);

            // Assert
            results.Should().BeEmpty();
        }

        [Fact]
        public void UpdateDynamicSucceeds()
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
                Email = "smith@bob.com"
            }, p => p.Email == "bob@bob.com");

            // Assert
            context.Should().NotBeNull();
            var updateContext = context as IUpdateQueryContext;
            updateContext.Updates.Should().HaveCount(1);
            updateContext.WhereClause.Should().BeOfType<state.Conditionals.Column<string>>();
            var values = updateContext.Updates;
            var value = values.First() as state.ValueSources.Update.StaticValueSource;
            value.Column.Should().NotBeNull();
            value.Column.Name.Should().Be(nameof(Person.Email));
            value.Value.Should().BeEquivalentTo("smith@bob.com");
        }

        [Fact]
        public void UpdateTypedSucceeds()
        {
            // Arrange
            IQueryContext context = null;
            this.personService.OnBeforeExecution += (sender, args) =>
            {
                context = args.Context;
            };

            // Act
            this.personService.Update(new Person
            {
                Id = 36,
                Email = "smith@bob.com"
            });

            // Assert
            context.Should().NotBeNull();
            var updateContext = context as IUpdateQueryContext;
            updateContext.Updates.Should().HaveCount(1);
            updateContext.WhereClause.Should().BeOfType<state.Conditionals.Column<int>>();
            var values = updateContext.Updates;
            values.Should().HaveCount(1);
            var value = values.First() as state.ValueSources.Update.StaticValueSource;
            value.Column.Name.Should().Be(nameof(Person.Email));
            value.GetValue<string>().Should().Be("smith@bob.com");
        }

        [Fact]
        public void DeleteSucceeds()
        {
            // Arrange
            IQueryContext context = null;
            this.personService.OnBeforeExecution += (sender, args) =>
            {
                context = args.Context;
            };
            const int key = 42;

            // Act
            this.personService.Delete(key);

            // Assert
            context.Should().NotBeNull();
            var deleteContext = context as IDeleteQueryContext;
            var valueClause = deleteContext.WhereClause as state.Conditionals.InValues<int>;
            valueClause.Should().NotBeNull();
            valueClause.Column.Name.Should().Be(nameof(Person.Id));
            valueClause.ValueSet.Should().Contain(key);
        }
    }
}

