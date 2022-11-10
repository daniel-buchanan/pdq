﻿using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using pdq.common;
using pdq.core_tests.Mocks;
using pdq.core_tests.Models;
using pdq.services;
using pdq.state;
using Xunit;

namespace pdq.core_tests.Services.Command
{
    public class CommandServiceWithKey1Tests
    {
        private readonly IService<Person, int> personService;

        public CommandServiceWithKey1Tests()
        {
            var services = new ServiceCollection();
            services.AddPdq(o =>
            {
                o.EnableTransientTracking();
                o.OverrideDefaultLogLevel(LogLevel.Debug);
                o.UseMockDatabase();
            });
            services.AddPdqService<Person, int>().AsScoped();
            services.AddScoped<IConnectionDetails, MockConnectionDetails>();

            var provider = services.BuildServiceProvider();
            this.personService = provider.GetService<IService<Person, int>>();
        }

        [Fact]
        public void AddSucceeds()
        {
            // Arrange
            IQueryContext context = null;
            this.personService.PreExecution += (sender, args) =>
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
            this.personService.PreExecution += (sender, args) =>
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
            this.personService.PreExecution += (sender, args) =>
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
            this.personService.PreExecution += (sender, args) =>
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
            this.personService.PreExecution += (sender, args) =>
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
            this.personService.PreExecution += (sender, args) =>
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
            this.personService.PreExecution += (sender, args) =>
            {
                context = args.Context;
            };

            // Act
            this.personService.Delete(p => p.Id == 42);

            // Assert
            context.Should().NotBeNull();
            var deleteContext = context as IDeleteQueryContext;
            deleteContext.WhereClause.Should().BeOfType<state.Conditionals.Column<int>>();
            var clause = deleteContext.WhereClause as state.Conditionals.Column<int>;
            clause.Details.Name.Should().Be(nameof(Person.Id));
            clause.Value.Should().Be(42);
        }
    }
}
