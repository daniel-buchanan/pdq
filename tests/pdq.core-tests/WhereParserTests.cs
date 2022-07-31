﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FluentAssertions;
using pdq.common;
using pdq.core_tests.Models;
using pdq.state;
using pdq.state.Conditionals;
using pdq.state.Conditionals.ValueFunctions;
using pdq.state.Utilities;
using pdq.state.Utilities.Parsers;
using Xunit;

namespace pdq.core_tests
{
    public class WhereParserTests
    {
        private readonly IAliasManager aliasManager;
        private readonly WhereParser parser;

        public WhereParserTests()
        {
            var reflectionHelper = new ReflectionHelper();
            var expressionHelper = new ExpressionHelper(reflectionHelper);
            this.aliasManager = AliasManager.Create();
            var callExpressionHelper = new CallExpressionHelper(expressionHelper);
            var valueParser = new ValueParser(expressionHelper, callExpressionHelper, reflectionHelper);
            var joinParser = new JoinParser(expressionHelper, reflectionHelper);
            this.parser = new WhereParser(expressionHelper, reflectionHelper, callExpressionHelper, joinParser, valueParser);
        }

        [Theory]
        [MemberData(nameof(ValueTests))]
        public void ParseSimpleExpressionSucceeds<T>(
            Expression expression,
            string expectedPropertyName,
            EqualityOperator expectedOperator,
            Func<T> getValue,
            Type functionType)
        {
            // Arrange
            var context = SelectQueryContext.Create(this.aliasManager) as IQueryContextInternal;
            context.AddQueryTarget(state.QueryTargets.TableTarget.Create(nameof(Person), "p"));

            // Act
            var result = this.parser.Parse(expression, context);

            // Assert
            var col = result as IColumn;
            col.Should().NotBeNull();
            col.Details.Name.Should().Be(expectedPropertyName);
            col.Details.Source.Alias.Should().Be("p");
            col.EqualityOperator.Should().Be(expectedOperator);
            col.Value.Should().Be(getValue());
            if(functionType != null)
            {
                col.ValueFunction.Should().BeAssignableTo(functionType);
            }
        }

        [Fact]
        public void ParseContainsExpressionSucceeds()
        {
            // Arrange
            var context = SelectQueryContext.Create(this.aliasManager) as IQueryContextInternal;
            context.AddQueryTarget(state.QueryTargets.TableTarget.Create(nameof(Person), "p"));
            Expression<Func<Person, bool>> expression = (p) => p.FirstName.Contains("smith");

            // Act
            var result = this.parser.Parse(expression, context);

            // Assert
            var col = result as IColumn;
            col.Should().NotBeNull();
            col.Details.Name.Should().Be(nameof(Person.FirstName));
            col.Details.Source.Alias.Should().Be("p");
            col.EqualityOperator.Should().Be(EqualityOperator.Equals);
            col.ValueFunction.Should().BeEquivalentTo(StringContains.Create("smith"));
            col.Value.Should().Be(true);
        }

        [Fact]
        public void ParseNotContainsExpressionSucceeds()
        {
            // Arrange
            var context = SelectQueryContext.Create(this.aliasManager) as IQueryContextInternal;
            context.AddQueryTarget(state.QueryTargets.TableTarget.Create(nameof(Person), "p"));
            Expression<Func<Person, bool>> expression = (p) => !p.FirstName.Contains("smith");

            // Act
            var result = this.parser.Parse(expression, context);

            // Assert
            var inversion = result as Not;
            inversion.Should().NotBeNull();
            var col = inversion.Item as IColumn;
            col.Should().NotBeNull();
            col.Details.Name.Should().Be(nameof(Person.FirstName));
            col.Details.Source.Alias.Should().Be("p");
            col.EqualityOperator.Should().Be(EqualityOperator.Equals);
            col.ValueFunction.Should().BeEquivalentTo(StringContains.Create("smith"));
            col.Value.Should().Be(true);
        }

        [Fact]
        public void ParseContainsEqualsFalseExpressionSucceeds()
        {
            // Arrange
            var context = SelectQueryContext.Create(this.aliasManager) as IQueryContextInternal;
            context.AddQueryTarget(state.QueryTargets.TableTarget.Create(nameof(Person), "p"));
            Expression<Func<Person, bool>> expression = (p) => p.FirstName.Contains("smith") == false;

            // Act
            var result = this.parser.Parse(expression, context);

            // Assert
            var inversion = result as Not;
            inversion.Should().NotBeNull();
            var col = inversion.Item as IColumn;
            col.Should().NotBeNull();
            col.Details.Name.Should().Be(nameof(Person.FirstName));
            col.Details.Source.Alias.Should().Be("p");
            col.EqualityOperator.Should().Be(EqualityOperator.Equals);
            col.ValueFunction.Should().BeEquivalentTo(StringContains.Create("smith"));
            col.Value.Should().Be(true);
        }

        public static IEnumerable<object[]> ValueTests
        {
            get
            {
                Func<string> getString = () => "john";
                Func<int> getInt = () => 42;
                Func<bool> getFalse = () => false;
                Func<bool> getTrue = () => true;

                yield return new object[]
                {
                    GetExpression<Person>((p) => p.FirstName == "john"),
                    nameof(Person.FirstName),
                    EqualityOperator.Equals,
                    getString,
                    null
                };

                yield return new object[]
                {
                    GetExpression<Person>((p) => p.FirstName != "john"),
                    nameof(Person.FirstName),
                    EqualityOperator.NotEquals,
                    getString,
                    null
                };

                yield return new object[]
                {
                    GetExpression<Person>((p) => p.AddressId == 42),
                    nameof(Person.AddressId),
                    EqualityOperator.Equals,
                    getInt,
                    null
                };

                yield return new object[]
                {
                    GetExpression<Person>((p) => p.AddressId != 42),
                    nameof(Person.AddressId),
                    EqualityOperator.NotEquals,
                    getInt,
                    null
                };

                yield return new object[]
                {
                    GetExpression<Person>((p) => p.AddressId > 42),
                    nameof(Person.AddressId),
                    EqualityOperator.GreaterThan,
                    getInt,
                    null
                };

                yield return new object[]
                {
                    GetExpression<Person>((p) => p.AddressId >= 42),
                    nameof(Person.AddressId),
                    EqualityOperator.GreaterThanOrEqualTo,
                    getInt,
                    null
                };

                yield return new object[]
                {
                    GetExpression<Person>((p) => p.AddressId < 42),
                    nameof(Person.AddressId),
                    EqualityOperator.LessThan,
                    getInt,
                    null
                };

                yield return new object[]
                {
                    GetExpression<Person>((p) => p.AddressId <= 42),
                    nameof(Person.AddressId),
                    EqualityOperator.LessThanOrEqualTo,
                    getInt,
                    null
                };

                yield return new object[]
                {
                    GetExpression<Person>(p => p.LastName.Contains("John")),
                    nameof(Person.LastName),
                    EqualityOperator.Equals,
                    getTrue,
                    typeof(StringContains)
                };
            }
        }

        private static Expression GetExpression<T>(Expression<Func<T, bool>> expression)
            => (Expression)expression;
    }
}

