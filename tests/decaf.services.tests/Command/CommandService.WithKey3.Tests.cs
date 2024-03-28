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
    public class CommandServiceWithKey3Tests
    {
        private readonly IService<AddressNote, int, int, int> addressNoteService;

        public CommandServiceWithKey3Tests()
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
            this.addressNoteService = provider.GetService<IService<AddressNote, int, int, int>>();
        }

        [Fact]
        public void AddSucceeds()
        {
            // Arrange
            IQueryContext context = null;
            this.addressNoteService.OnBeforeExecution += (sender, args) =>
            {
                context = args.Context;
            };

            // Act
            this.addressNoteService.Add(new AddressNote
            {
                PersonId = 1,
                Id = 42,
                AddressId = 1,
                Value = "Hello"
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
            this.addressNoteService.OnBeforeExecution += (sender, args) =>
            {
                context = args.Context;
            };

            // Act
            this.addressNoteService.Add(new AddressNote
            {
                PersonId = 1,
                Id = 42,
                Value = "Hamilton"
            },
            new AddressNote
            {
                PersonId = 1,
                Id = 43,
                Value = "Hamilton"
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
            this.addressNoteService.OnBeforeExecution += (sender, args) =>
            {
                context = args.Context;
            };

            // Act
            this.addressNoteService.Add(new List<AddressNote> {
                new AddressNote
                {
                    PersonId = 1,
                    Id = 42,
                    AddressId = 1,
                    Value = "Hamilton"
                },
                new AddressNote {
                    PersonId = 1,
                    Id = 43,
                    AddressId = 2,
                    Value = "Hamilton"
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
            this.addressNoteService.OnBeforeExecution += (sender, args) =>
            {
                context = args.Context;
            };
            var items = Enumerable.Empty<AddressNote>();

            // Act
            var results = this.addressNoteService.Add(items);

            // Assert
            results.Should().BeEmpty();
        }

        [Fact]
        public void UpdateDynamicSucceeds()
        {
            // Arrange
            IQueryContext context = null;
            this.addressNoteService.OnBeforeExecution += (sender, args) =>
            {
                context = args.Context;
            };

            // Act
            this.addressNoteService.Update(new
            {
                Value = "Auckland"
            }, p => p.Value == "3216");

            // Assert
            context.Should().NotBeNull();
            var updateContext = context as IUpdateQueryContext;
            updateContext.Updates.Should().HaveCount(1);
            updateContext.WhereClause.Should().BeOfType<state.Conditionals.Column<string>>();
            var values = updateContext.Updates;
            var value = values.First() as state.ValueSources.Update.StaticValueSource;
            value.Column.Should().NotBeNull();
            value.Column.Name.Should().Be(nameof(AddressNote.Value));
            value.Value.Should().BeEquivalentTo("Auckland");
        }

        [Fact]
        public void UpdateTypedSucceeds()
        {
            // Arrange
            IQueryContext context = null;
            this.addressNoteService.OnBeforeExecution += (sender, args) =>
            {
                context = args.Context;
            };

            // Act
            this.addressNoteService.Update(new AddressNote
            {
                Id = 36,
                PersonId = 12,
                AddressId = 2,
                Value = "Waikato"
            });

            // Assert
            context.Should().NotBeNull();
            var updateContext = context as IUpdateQueryContext;
            updateContext.Updates.Should().HaveCount(1);
            updateContext.WhereClause.Should().BeOfType<state.Conditionals.And>();
            var values = updateContext.Updates;
            values.Should().HaveCount(1);
            var value = values.First() as state.ValueSources.Update.StaticValueSource;
            value.Column.Name.Should().Be(nameof(AddressNote.Value));
            value.GetValue<string>().Should().Be("Waikato");
        }

        [Fact]
        public void DeleteSucceeds()
        {
            // Arrange
            // Arrange
            IQueryContext context = null;
            this.addressNoteService.OnBeforeExecution += (sender, args) =>
            {
                context = args.Context;
            };

            // Act
            this.addressNoteService.Delete(42, 43, 44);

            // Assert
            context.Should().NotBeNull();
            var deleteContext = context as IDeleteQueryContext;
            deleteContext.WhereClause.Should().BeOfType<state.Conditionals.And>();
            var clause = deleteContext.WhereClause as state.Conditionals.And;
            clause.Children.Should().HaveCount(3);
            var one = clause.Children.ToArray()[0] as state.Conditionals.Column<int>;
            var two = clause.Children.ToArray()[1] as state.Conditionals.Column<int>;
            var three = clause.Children.ToArray()[2] as state.Conditionals.Column<int>;

            one.Should().NotBeNull();
            one.Details.Name.Should().Be(nameof(AddressNote.Id));
            one.Value.Should().Be(42);

            two.Should().NotBeNull();
            two.Details.Name.Should().Be(nameof(AddressNote.PersonId));
            two.Value.Should().Be(43);

            three.Should().NotBeNull();
            three.Details.Name.Should().Be(nameof(AddressNote.AddressId));
            three.Value.Should().Be(44);
        }
    }
}

