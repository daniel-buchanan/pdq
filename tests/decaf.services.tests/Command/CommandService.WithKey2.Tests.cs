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
    public class CommandServiceWithKey2Tests
    {
        private readonly IService<Address, int, int> addressService;

        public CommandServiceWithKey2Tests()
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
        public void AddSucceeds()
        {
            // Arrange
            IQueryContext context = null;
            this.addressService.OnBeforeExecution += (sender, args) =>
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
            this.addressService.OnBeforeExecution += (sender, args) =>
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
            new Address
            {
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
            this.addressService.OnBeforeExecution += (sender, args) =>
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
            this.addressService.OnBeforeExecution += (sender, args) =>
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
            this.addressService.OnBeforeExecution += (sender, args) =>
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
            this.addressService.OnBeforeExecution += (sender, args) =>
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
            this.addressService.OnBeforeExecution += (sender, args) =>
            {
                context = args.Context;
            };

            // Act
            this.addressService.Delete(42, 43);

            // Assert
            context.Should().NotBeNull();
            var deleteContext = context as IDeleteQueryContext;
            deleteContext.WhereClause.Should().BeOfType<state.Conditionals.And>();
            var clause = deleteContext.WhereClause as state.Conditionals.And;
            clause.Children.Should().HaveCount(2);
            var left = clause.Children.ToArray()[0] as state.Conditionals.Column<int>;
            var right = clause.Children.ToArray()[1] as state.Conditionals.Column<int>;

            left.Should().NotBeNull();
            left.Details.Name.Should().Be(nameof(Address.Id));
            left.Value.Should().Be(42);

            right.Should().NotBeNull();
            right.Details.Name.Should().Be(nameof(Address.PersonId));
            right.Value.Should().Be(43);
        }
    }
}

