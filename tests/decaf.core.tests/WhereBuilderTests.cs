using System;
using System.Collections.Generic;
using System.Linq;
using decaf.common;
using decaf.common.Utilities;
using decaf.Exceptions;
using decaf.Implementation;
using decaf.state;
using decaf.state.Conditionals;
using FluentAssertions;
using Moq;
using decaf.common.Logging;
using decaf.tests.common.Mocks;
using Xunit;
using decaf.services;
using decaf.common.Connections;
using decaf.common.Utilities.Reflection;
using Microsoft.Extensions.DependencyInjection;
using decaf.db.common;

namespace decaf.core_tests
{
    public class WhereBuilderTests
    {
        private DecafOptions options;
        private readonly ISelectQueryContext context;
        private readonly Select select;

        public WhereBuilderTests()
        {
            var services = new ServiceCollection();
            services.AddDecaf(b =>
            {
                b.UseMockDatabase().WithMockConnectionDetails();
            });
            var provider = services.BuildServiceProvider();
            var decaf = provider.GetRequiredService<IDecaf>();
            this.options = provider.GetRequiredService<DecafOptions>();
            var query = decaf.Query();
            var aliasManager = AliasManager.Create();
            var hashProvider = new HashProvider();
            this.context = SelectQueryContext.Create(aliasManager, hashProvider);
            this.select = Select.Create(this.context, query);
        }

        [Theory]
        [MemberData(nameof(EqualToTests))]
        public void AndSucceeds<T>(Func<T> getValue)
        {
            // Arrange
            select.From("person", "p");
            var value = getValue();

            // Act
            select.Where(b =>
            {
                b.And(a =>
                {
                    a.Column("name", "p").Is().EqualTo(value);
                    a.Column("name", "p").IsNot().EqualTo(value);
                });
            });

            // Assert
            var and = this.context.WhereClause as And;
            and.Should().NotBeNull();
            and.Children.Should().HaveCount(2);
        }

        [Theory]
        [MemberData(nameof(EqualToTests))]
        public void OrSucceeds<T>(Func<T> getValue)
        {
            // Arrange
            select.From("person", "p");
            var value = getValue();

            // Act
            select.Where(b =>
            {
                b.Or(a =>
                {
                    a.Column("name", "p").Is().EqualTo(value);
                    a.Column("name", "p").IsNot().EqualTo(value);
                });
            });

            // Assert
            var or = this.context.WhereClause as Or;
            or.Should().NotBeNull();
            or.Children.Should().HaveCount(2);
        }

        [Theory]
        [MemberData(nameof(EqualToTests))]
        public void WithoutAliasEqualTo<T>(Func<T> getValue)
        {
            // Arrange
            select.From("person", "p");
            var value = getValue();

            // Act
            select.Where(b =>
            {
                b.Column("name").Is().EqualTo(value);
            });

            // Assert
            var column = this.context.WhereClause as IColumn;
            column.Should().NotBeNull();
            column.Details.Name.Should().Be("name");
            column.Value.Should().Be(value);
            column.ValueType.Should().Be(typeof(T));
            column.EqualityOperator.Should().Be(EqualityOperator.Equals);
        }

        [Theory]
        [MemberData(nameof(EqualToTests))]
        public void EqualTo<T>(Func<T> getValue)
        {
            // Arrange
            select.From("person", "p");
            var value = getValue();

            // Act
            select.Where(b =>
            {
                b.Column("name", "p").Is().EqualTo(value);
            });

            // Assert
            var column = this.context.WhereClause as IColumn;
            column.Should().NotBeNull();
            column.Details.Name.Should().Be("name");
            column.Details.Source.Alias.Should().Be("p");
            column.Value.Should().Be(value);
            column.ValueType.Should().Be(typeof(T));
            column.EqualityOperator.Should().Be(EqualityOperator.Equals);
        }

        [Theory]
        [MemberData(nameof(EqualToTests))]
        public void NotEqualTo<T>(Func<T> getValue)
        {
            // Arrange
            select.From("person", "p");
            var value = getValue();

            // Act
            select.Where(b =>
            {
                b.Column("name", "p").IsNot().EqualTo(value);
            });

            // Assert
            var inversion = this.context.WhereClause as Not;
            inversion.Should().NotBeNull();
            var column = inversion.Item as IColumn;
            column.Should().NotBeNull();
            column.Details.Name.Should().Be("name");
            column.Details.Source.Alias.Should().Be("p");
            column.Value.Should().Be(value);
            column.ValueType.Should().Be(typeof(T));
            column.EqualityOperator.Should().Be(EqualityOperator.Equals);
        }

        [Theory]
        [MemberData(nameof(NonStringTests))]
        public void LessThan<T>(Func<T> getValue)
            where T : struct
        {
            // Arrange
            select.From("person", "p");
            var value = getValue();

            // Act
            select.Where(b =>
            {
                b.Column("name", "p").Is().LessThan(value);
            });

            // Assert
            var column = this.context.WhereClause as IColumn;
            column.Should().NotBeNull();
            column.Details.Name.Should().Be("name");
            column.Details.Source.Alias.Should().Be("p");
            column.Value.Should().Be(value);
            column.ValueType.Should().Be(typeof(T));
            column.EqualityOperator.Should().Be(EqualityOperator.LessThan);
        }

        [Theory]
        [MemberData(nameof(NonStringTests))]
        public void NotLessThan<T>(Func<T> getValue)
            where T : struct
        {
            // Arrange
            select.From("person", "p");
            var value = getValue();

            // Act
            select.Where(b =>
            {
                b.Column("name", "p").IsNot().LessThan(value);
            });

            // Assert
            var inversion = this.context.WhereClause as Not;
            inversion.Should().NotBeNull();
            var column = inversion.Item as IColumn;
            column.Should().NotBeNull();
            column.Details.Name.Should().Be("name");
            column.Details.Source.Alias.Should().Be("p");
            column.Value.Should().Be(value);
            column.ValueType.Should().Be(typeof(T));
            column.EqualityOperator.Should().Be(EqualityOperator.LessThan);
        }

        [Theory]
        [MemberData(nameof(NonStringTests))]
        public void LessThanOrEqualTo<T>(Func<T> getValue)
            where T : struct
        {
            // Arrange
            select.From("person", "p");
            var value = getValue();

            // Act
            select.Where(b =>
            {
                b.Column("name", "p").Is().LessThanOrEqualTo(value);
            });

            // Assert
            var column = this.context.WhereClause as IColumn;
            column.Should().NotBeNull();
            column.Details.Name.Should().Be("name");
            column.Details.Source.Alias.Should().Be("p");
            column.Value.Should().Be(value);
            column.ValueType.Should().Be(typeof(T));
            column.EqualityOperator.Should().Be(EqualityOperator.LessThanOrEqualTo);
        }

        [Theory]
        [MemberData(nameof(NonStringTests))]
        public void NotLessThanOrEqualTo<T>(Func<T> getValue)
            where T : struct
        {
            // Arrange
            select.From("person", "p");
            var value = getValue();

            // Act
            select.Where(b =>
            {
                b.Column("name", "p").IsNot().LessThanOrEqualTo(value);
            });

            // Assert
            var inversion = this.context.WhereClause as Not;
            inversion.Should().NotBeNull();
            var column = inversion.Item as IColumn;
            column.Should().NotBeNull();
            column.Details.Name.Should().Be("name");
            column.Details.Source.Alias.Should().Be("p");
            column.Value.Should().Be(value);
            column.ValueType.Should().Be(typeof(T));
            column.EqualityOperator.Should().Be(EqualityOperator.LessThanOrEqualTo);
        }

        [Theory]
        [MemberData(nameof(NonStringTests))]
        public void GreaterThan<T>(Func<T> getValue)
            where T : struct
        {
            // Arrange
            select.From("person", "p");
            var value = getValue();

            // Act
            select.Where(b =>
            {
                b.Column("name", "p").Is().GreaterThan(value);
            });

            // Assert
            var column = this.context.WhereClause as IColumn;
            column.Should().NotBeNull();
            column.Details.Name.Should().Be("name");
            column.Details.Source.Alias.Should().Be("p");
            column.Value.Should().Be(value);
            column.ValueType.Should().Be(typeof(T));
            column.EqualityOperator.Should().Be(EqualityOperator.GreaterThan);
        }

        [Theory]
        [MemberData(nameof(NonStringTests))]
        public void GreaterThanOrEqualTo<T>(Func<T> getValue)
            where T : struct
        {
            // Arrange
            select.From("person", "p");
            var value = getValue();

            // Act
            select.Where(b =>
            {
                b.Column("name", "p").Is().GreaterThanOrEqualTo(value);
            });

            // Assert
            var column = this.context.WhereClause as IColumn;
            column.Should().NotBeNull();
            column.Details.Name.Should().Be("name");
            column.Details.Source.Alias.Should().Be("p");
            column.Value.Should().Be(value);
            //column.ValueType.Should().Be(type);
            column.EqualityOperator.Should().Be(EqualityOperator.GreaterThanOrEqualTo);
        }

        [Theory]
        [MemberData(nameof(NonStringTests))]
        public void Between<T>(Func<T> getValue)
            where T : struct
        {
            // Arrange
            select.From("person", "p");
            var value = getValue();

            // Act
            select.Where(b =>
            {
                b.Column("name", "p").Is().Between(value, value);
            });

            // Assert
            var column = this.context.WhereClause as IBetween;
            column.Should().NotBeNull();
            column.Column.Name.Should().Be("name");
            column.Column.Source.Alias.Should().Be("p");
            column.Start.Should().Be(value);
            column.End.Should().Be(value);
            column.ValueType.Should().Be(typeof(T));
        }

        [Theory]
        [MemberData(nameof(NonStringTests))]
        public void NotBetween<T>(Func<T> getValue)
            where T : struct
        {
            // Arrange
            select.From("person", "p");
            var value = getValue();

            // Act
            select.Where(b =>
            {
                b.Column("name", "p").IsNot().Between(value, value);
            });

            // Assert
            var inversion = this.context.WhereClause as Not;
            inversion.Should().NotBeNull();
            var column = inversion.Item as IBetween;
            column.Should().NotBeNull();
            column.Column.Name.Should().Be("name");
            column.Column.Source.Alias.Should().Be("p");
            column.Start.Should().Be(value);
            column.End.Should().Be(value);
            column.ValueType.Should().Be(typeof(T));
        }

        [Theory]
        [MemberData(nameof(NonStringTests))]
        public void IsInParams<T>(Func<T> getValue)
            where T : struct
        {
            // Arrange
            select.From("person", "p");
            var value = getValue();

            // Act
            select.Where(b =>
            {
                b.Column("name", "p").Is().In(value, value, value, value);
            });

            // Assert
            var column = this.context.WhereClause as IInValues;
            column.Should().NotBeNull();
            column.Column.Name.Should().Be("name");
            column.Column.Source.Alias.Should().Be("p");
            column.GetValues().Should().HaveCount(4);
            column.ValueType.Should().Be(typeof(T));
            var typedColumn = column as InValues<T>;
            typedColumn.Should().NotBeNull();
            typedColumn.ValueSet.Should().HaveCount(4);
        }

        [Theory]
        [MemberData(nameof(NonStringTests))]
        public void IsInEnumerable<T>(Func<T> getValue)
            where T : struct
        {
            // Arrange
            select.From("person", "p");
            var value = getValue();
            var values = new List<T> { value, value, value, value };

            // Act
            select.Where(b =>
            {
                b.Column("name", "p").Is().In<T>(values);
            });

            // Assert
            var column = this.context.WhereClause as IInValues;
            column.Should().NotBeNull();
            column.Column.Name.Should().Be("name");
            column.Column.Source.Alias.Should().Be("p");
            column.GetValues().Should().HaveCount(4);
            column.ValueType.Should().Be(typeof(T));
            var typedColumn = column as InValues<T>;
            typedColumn.Should().NotBeNull();
            typedColumn.ValueSet.Should().HaveCount(4);
        }

        [Theory]
        [MemberData(nameof(NonStringTests))]
        public void IsInNullEnumerable<T>(Func<T> getValue)
            where T : struct
        {
            // Arrange
            select.From("person", "p");
            var value = getValue();
            List<T> values = null;

            // Act
            select.Where(b =>
            {
                b.Column("name", "p").Is().In<T>(values);
            });

            // Assert
            var column = this.context.WhereClause as IInValues;
            column.Should().NotBeNull();
            column.Column.Name.Should().Be("name");
            column.Column.Source.Alias.Should().Be("p");
            column.GetValues().Should().HaveCount(0);
            column.ValueType.Should().Be(typeof(T));
        }

        [Theory]
        [MemberData(nameof(EqualToTests))]
        public void StartsWith<T>(Func<T> getValue)
        {
            // Arrange
            select.From("person", "p");
            var value = getValue();

            // Act
            select.Where(b =>
            {
                b.Column("name", "p").Is().StartsWith(value);
            });

            // Assert
            var column = this.context.WhereClause as IColumn;
            column.Should().NotBeNull();
            column.Details.Name.Should().Be("name");
            column.Details.Source.Alias.Should().Be("p");
            column.Value.Should().Be(value);
            column.ValueType.Should().Be(typeof(T));
            column.EqualityOperator.Should().Be(EqualityOperator.StartsWith);
        }

        [Theory]
        [MemberData(nameof(EqualToTests))]
        public void EndsWith<T>(Func<T> getValue)
        {
            // Arrange
            select.From("person", "p");
            var value = getValue();

            // Act
            select.Where(b =>
            {
                b.Column("name", "p").Is().EndsWith(value);
            });

            // Assert
            var column = this.context.WhereClause as IColumn;
            column.Should().NotBeNull();
            column.Details.Name.Should().Be("name");
            column.Details.Source.Alias.Should().Be("p");
            column.Value.Should().Be(value);
            column.ValueType.Should().Be(typeof(T));
            column.EqualityOperator.Should().Be(EqualityOperator.EndsWith);
        }

        [Theory]
        [MemberData(nameof(EqualToTests))]
        public void Like<T>(Func<T> getValue)
        {
            // Arrange
            select.From("person", "p");
            var value = getValue();

            // Act
            select.Where(b =>
            {
                b.Column("name", "p").Is().Like(value);
            });

            // Assert
            var column = this.context.WhereClause as IColumn;
            column.Should().NotBeNull();
            column.Details.Name.Should().Be("name");
            column.Details.Source.Alias.Should().Be("p");
            column.Value.Should().Be(value);
            column.ValueType.Should().Be(typeof(T));
            column.EqualityOperator.Should().Be(EqualityOperator.Like);
        }

        [Theory]
        [MemberData(nameof(EqualToTests))]
        public void NotLike<T>(Func<T> getValue)
        {
            // Arrange
            select.From("person", "p");
            var value = getValue();

            // Act
            select.Where(b =>
            {
                b.Column("name", "p").IsNot().Like(value);
            });

            // Assert
            var inversion = this.context.WhereClause as Not;
            inversion.Should().NotBeNull();
            var column = inversion.Item as IColumn;
            column.Should().NotBeNull();
            column.Details.Name.Should().Be("name");
            column.Details.Source.Alias.Should().Be("p");
            column.Value.Should().Be(value);
            column.ValueType.Should().Be(typeof(T));
            column.EqualityOperator.Should().Be(EqualityOperator.Like);
        }

        [Fact]
        public void ClauseHandlingChangeToAndSucceeds()
        {
            // Arrange
            select.From("person", "p");

            // Act
            IWhereBuilderInternal builder = null;
            select.Where(b =>
            {
                b.ClauseHandling.DefaultToAnd();
                builder = b as IWhereBuilderInternal;
            });

            // Assert
            builder.DefaultClauseHandling.Should().Be(ClauseHandling.And);
        }

        [Fact]
        public void ClauseHandlingChangeToOrSucceeds()
        {
            // Arrange
            select.From("person", "p");

            // Act
            IWhereBuilderInternal builder = null;
            select.Where(b =>
            {
                b.ClauseHandling.DefaultToOr();
                builder = b as IWhereBuilderInternal;
            });

            // Assert
            builder.DefaultClauseHandling.Should().Be(ClauseHandling.Or);
        }

        [Fact]
        public void NoClauseHandlingShouldThrowException()
        {
            // Arrange
            this.options.SetProperty(o => o.DefaultClauseHandling, ClauseHandling.Unspecified);
            select.From("person", "p");

            // Act
            Action method = () => select.Where(b =>
            {
                b.Column("name", "p").Is().EqualTo("smith");
            });

            // Assert
            method.Should().Throw<WhereBuildFailedException>();
        }

        public static IEnumerable<object[]> EqualToTests
        {
            get
            {
                yield return new object[]
                {
                    (Func<string>)(() => GetValue<string>("smith"))
                };

                foreach (var r in NonStringTests)
                    yield return r;
            }
        }

        public static IEnumerable<object[]> NonStringTests
        {
            get
            {

                yield return new object[]
                {
                    (Func<int>)(() => GetValue<int>(42))
                };

                yield return new object[]
                {
                    (Func<double>)(() => GetValue<double>(42))
                };

                yield return new object[]
                {
                    (Func<float>)(() => GetValue<float>(42))
                };

                yield return new object[]
                {
                    (Func<decimal>)(() => GetValue<decimal>(42))
                };

                yield return new object[]
                {
                    (Func<short>)(() => GetValue<short>(42))
                };

                yield return new object[]
                {
                    (Func<uint>)(() => GetValue<uint>(42))
                };

                yield return new object[]
                {
                    (Func<DateTime>)(() => GetValue<DateTime>(DateTime.UtcNow))
                };
            }
        }

        static T GetValue<T>(T defaultValue) => defaultValue;
    }
}

