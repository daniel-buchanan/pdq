using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FluentAssertions;
using pdq.common;
using pdq.core_tests.Models;
using pdq.state;
using pdq.state.Conditionals;
using pdq.state.Utilities;
using pdq.state.Utilities.Parsers;
using Xunit;

namespace pdq.core_tests
{
    public class JoinParserTests
    {
        private readonly IAliasManager aliasManager;
        private readonly JoinParser parser;

        public JoinParserTests()
        {
            var reflectionHelper = new ReflectionHelper();
            var expressionHelper = new ExpressionHelper(reflectionHelper);
            this.aliasManager = AliasManager.Create();
            var callExpressionHelper = new CallExpressionHelper(expressionHelper);
            var valueParser = new ValueParser(expressionHelper, callExpressionHelper, reflectionHelper);
            var joinParser = new JoinParser(expressionHelper, reflectionHelper);
            this.parser = new JoinParser(expressionHelper, reflectionHelper);
        }

        [Fact]
        public void ParseSimpleJoinSucceeds()
        {
            // Arrange
            var context = SelectQueryContext.Create(this.aliasManager) as IQueryContextInternal;
            context.AddQueryTarget(state.QueryTargets.TableTarget.Create(nameof(Person), "p"));
            Expression<Func<Person, Address, bool>> expression = (p, a) => p.AddressId == a.Id;

            // Act
            var result = this.parser.Parse(expression, context);

            // Assert
            result.Should().NotBeNull();
            var match = result as ColumnMatch;
            match.Left.Should().NotBeNull();
            match.Left.Name.Should().Be(nameof(Person.AddressId));
            match.Left.Source.Alias.Should().Be("p");
            match.Right.Should().NotBeNull();
            match.Right.Name.Should().Be(nameof(Address.Id));
            match.Right.Source.Alias.Should().Be("a");
        }

        [Fact]
        public void ParseReverseSimpleJoinSucceeds()
        {
            // Arrange
            var context = SelectQueryContext.Create(this.aliasManager) as IQueryContextInternal;
            context.AddQueryTarget(state.QueryTargets.TableTarget.Create(nameof(Person), "p"));
            Expression<Func<Address, Person, bool>> expression = (a, p) => a.Id == p.AddressId;

            // Act
            var result = this.parser.Parse(expression, context);

            // Assert
            result.Should().NotBeNull();
            var match = result as ColumnMatch;
            match.Right.Should().NotBeNull();
            match.Right.Name.Should().Be(nameof(Person.AddressId));
            match.Right.Source.Alias.Should().Be("p");
            match.Left.Should().NotBeNull();
            match.Left.Name.Should().Be(nameof(Address.Id));
            match.Left.Source.Alias.Should().Be("a");
        }

        private static Expression GetExpression<T>(Expression<Func<T, bool>> expression)
            => (Expression)expression;
    }
}

