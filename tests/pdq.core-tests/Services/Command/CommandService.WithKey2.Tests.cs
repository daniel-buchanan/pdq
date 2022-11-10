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
    public class CommandServiceWithKey2Tests
    {
        private readonly IService<Address, int, int> addressService;

        public CommandServiceWithKey2Tests()
        {
            var services = new ServiceCollection();
            services.AddPdq(o =>
            {
                o.EnableTransientTracking();
                o.OverrideDefaultLogLevel(LogLevel.Debug);
                o.UseMockDatabase();
            });
            services.AddPdqService<Address, int, int>().AsScoped();
            services.AddScoped<IConnectionDetails, MockConnectionDetails>();

            var provider = services.BuildServiceProvider();
            this.addressService = provider.GetService<IService<Address, int, int>>();
        }

        [Fact]
        public void AddSucceeds()
        {
            // Arrange
            IQueryContext context = null;
            this.addressService.PreExecution += (sender, args) =>
            {
                context = args.Context;
            };

            // Act
            this.addressService.Add(new Address
            {
                PersonId = 1,
                Id = 42,
                City = "Hamilton"
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
            this.addressService.PreExecution += (sender, args) =>
            {
                context = args.Context;
            };

            // Act
            this.addressService.Add(new Address
            {
                PersonId = 1,
                Id = 42,
                City = "Hamilton"
            },
            new Address {
                PersonId = 1,
                Id = 43,
                City = "Hamilton"
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
            this.addressService.PreExecution += (sender, args) =>
            {
                context = args.Context;
            };

            // Act
            this.addressService.Add(new List<Address> {
                new Address
                {
                    PersonId = 1,
                    Id = 42,
                    City = "Hamilton"
                },
                new Address {
                    PersonId = 1,
                    Id = 43,
                    City = "Hamilton"
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
            this.addressService.PreExecution += (sender, args) =>
            {
                context = args.Context;
            };
            var items = Enumerable.Empty<Address>();

            // Act
            var results = this.addressService.Add(items);

            // Assert
            results.Should().BeEmpty();
        }

        [Fact]
        public void UpdateDynamicSucceeds()
        {
            // Arrange
            IQueryContext context = null;
            this.addressService.PreExecution += (sender, args) =>
            {
                context = args.Context;
            };

            // Act
            this.addressService.Update(new
            {
                City = "Auckland"
            }, p => p.PostCode == "3216");

            // Assert
            context.Should().NotBeNull();
            var updateContext = context as IUpdateQueryContext;
            updateContext.Updates.Should().HaveCount(1);
            updateContext.WhereClause.Should().BeOfType<state.Conditionals.Column<string>>();
            var values = updateContext.Updates;
            var value = values.First() as state.ValueSources.Update.StaticValueSource;
            value.Column.Should().NotBeNull();
            value.Column.Name.Should().Be(nameof(Address.City));
            value.Value.Should().BeEquivalentTo("Auckland");
        }

        [Fact]
        public void UpdateTypedSucceeds()
        {
            // Arrange
            IQueryContext context = null;
            this.addressService.PreExecution += (sender, args) =>
            {
                context = args.Context;
            };

            // Act
            this.addressService.Update(new Address
            {
                Id = 36,
                PersonId = 12,
                Region = "Waikato"
            });

            // Assert
            context.Should().NotBeNull();
            var updateContext = context as IUpdateQueryContext;
            updateContext.Updates.Should().HaveCount(1);
            updateContext.WhereClause.Should().BeOfType<state.Conditionals.And>();
            var values = updateContext.Updates;
            values.Should().HaveCount(1);
            var value = values.First() as state.ValueSources.Update.StaticValueSource;
            value.Column.Name.Should().Be(nameof(Address.Region));
            value.GetValue<string>().Should().Be("Waikato");
        }

        [Fact]
        public void DeleteSucceeds()
        {
            // Arrange
            IQueryContext context = null;
            this.addressService.PreExecution += (sender, args) =>
            {
                context = args.Context;
            };

            // Act
            this.addressService.Delete(p => p.Id == 42);

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
