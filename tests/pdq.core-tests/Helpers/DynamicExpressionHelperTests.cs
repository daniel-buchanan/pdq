using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FluentAssertions;
using pdq.common;
using pdq.core_tests.Models;
using pdq.state;
using pdq.state.Utilities;
using Xunit;

namespace pdq.core_tests.Helpers
{
    public class DynamicExpressionHelperTests
    {
        private readonly IDynamicExpressionHelper dynamicExpressionHelper;
        private readonly IQueryContextInternal queryContext;

        public DynamicExpressionHelperTests()
        {
            var aliasManager = AliasManager.Create();
            var reflectionHelper = new ReflectionHelper();
            var expressionHelper = new ExpressionHelper(reflectionHelper);
            var callExpressionHelper = new CallExpressionHelper(expressionHelper);
            this.dynamicExpressionHelper = new DynamicExpressionHelper(expressionHelper, callExpressionHelper);
            this.queryContext = SelectQueryContext.Create(aliasManager) as IQueryContextInternal;
        }

        [Fact]
        public void ParseDynamicNoAssignmentSucceeds()
        {
            // Arrange
            var func = GetDynamicExpr<ParamItem>((p) => new
            {
                p.City,
                p.Age
            });

            // Act
            var properties = this.dynamicExpressionHelper.GetProperties(func, this.queryContext);

            // Assert
            properties.Should().HaveCount(2);
            properties.Should().ContainEquivalentOf(DynamicPropertyInfo.Create("Age", type: typeof(ParamItem), alias: "p"));
            properties.Should().ContainEquivalentOf(DynamicPropertyInfo.Create("City", type: typeof(ParamItem), alias: "p"));
        }

        [Fact]
        public void ParseDynamicWithNewNameSucceeds()
        {
            // Arrange
            var func = GetDynamicExpr<ParamItem>((p) => new
            {
                CityName = p.City,
                p.Age
            });

            // Act
            var properties = this.dynamicExpressionHelper.GetProperties(func, this.queryContext);

            // Assert
            properties.Should().HaveCount(2);
            properties.Should().ContainEquivalentOf(DynamicPropertyInfo.Create("Age", type: typeof(ParamItem), alias: "p"));
            properties.Should().ContainEquivalentOf(DynamicPropertyInfo.Create("City", type: typeof(ParamItem), alias: "p", newName: "CityName"));
        }

        [Fact]
        public void ParseConcreteSucceeds()
        {
            // Arrange
            var func = GetConcreteExpr<SourceItem, ParamItem>((s) => new ParamItem
            {
                City = s.City,
                Age = s.Age,
                Name = s.FullName
            });

            // Act
            var properties = this.dynamicExpressionHelper.GetProperties(func, this.queryContext);

            // Assert
            properties.Should().HaveCount(3);
            properties.Should().ContainEquivalentOf(DynamicPropertyInfo.Create("Age", alias: "s", type: typeof(int), newName: "Age"));
            properties.Should().ContainEquivalentOf(DynamicPropertyInfo.Create("City", alias: "s", type: typeof(string), newName: "City"));
            properties.Should().ContainEquivalentOf(DynamicPropertyInfo.Create("FullName", alias: "s", type: typeof(string), newName: "Name"));
        }

        [Theory]
        [MemberData(nameof(ValidExpressions))]
        public void ParseExpressionDynamicSucceeds(Expression expression, IEnumerable<DynamicPropertyInfo> expected)
        {
            // Act
            var results = this.dynamicExpressionHelper.GetProperties(expression, this.queryContext);

            // Assert
            results.Should().BeEquivalentTo(expected);
        }

        public static IEnumerable<object[]> ValidExpressions
        {
            get
            {
                yield return new object[]
                {
                    GetDynamicExpr((b) => new
                    {
                        Name = b.Is("name"),
                        City = b.Is("city"),
                        Region = b.Is("region_name", "r")
                    }),
                    new DynamicPropertyInfo[]
                    {
                        DynamicPropertyInfo.Create("name", "Name", type: typeof(object)),
                        DynamicPropertyInfo.Create("city", "City", type: typeof(object)),
                        DynamicPropertyInfo.Create("region_name", "Region", alias: "r", type: typeof(object))
                    }
                };

                yield return new object[]
                {
                    GetDynamicExpr<Person>((p) => new
                    {
                        Name = p.FirstName,
                        Email = p.Email,
                        Region = p.AddressId
                    }),
                    new DynamicPropertyInfo[]
                    {
                        DynamicPropertyInfo.Create(name: nameof(Person.AddressId), newName: "Region", alias: "p", type: typeof(Person)),
                        DynamicPropertyInfo.Create(name: nameof(Person.Email), newName: null, alias: "p", type: typeof(Person)),
                        DynamicPropertyInfo.Create(name: nameof(Person.FirstName), newName: "Name", alias: "p", type: typeof(Person))
                    }
                };

                yield return new object[]
                {
                    GetDynamicExpr<Person>((p) => new
                    {
                        p.FirstName,
                        p.Email,
                        Region = p.AddressId
                    }),
                    new DynamicPropertyInfo[]
                    {
                        DynamicPropertyInfo.Create(name: nameof(Person.FirstName), newName: null, alias: "p", type: typeof(Person)),
                        DynamicPropertyInfo.Create(name: nameof(Person.Email), newName: null, alias: "p", type: typeof(Person)),
                        DynamicPropertyInfo.Create(name: nameof(Person.AddressId), newName: "Region", alias: "p", type: typeof(Person))
                    }
                };

                yield return new object[]
                {
                    GetDynamicExpr<Person>((p) => new
                    {
                        p.FirstName,
                        p.Email,
                        Region = p.AddressId,
                        Timestamp = p.CreatedAt.DatePart(DatePart.Epoch)
                    }),
                    new DynamicPropertyInfo[]
                    {
                        DynamicPropertyInfo.Create(name: nameof(Person.FirstName), newName: null, alias: "p", type: typeof(Person)),
                        DynamicPropertyInfo.Create(name: nameof(Person.Email), newName: null, alias: "p", type: typeof(Person)),
                        DynamicPropertyInfo.Create(name: nameof(Person.AddressId), newName: "Region", alias: "p", type: typeof(Person)),
                        DynamicPropertyInfo.Create(name: nameof(Person.CreatedAt), newName: "Timestamp", alias: "p", type: typeof(Person), function: state.Conditionals.ValueFunctions.DatePart.Create(DatePart.Epoch))
                    }
                };
            }
        }

        public static Expression GetDynamicExpr(Expression<Func<ISelectColumnBuilder, dynamic>> expression)
            => expression;

        public static Expression GetConcreteExpr<T>(Expression<Func<ISelectColumnBuilder, T>> expression)
            => expression;

        public static Expression GetDynamicExpr<TSource>(Expression<Func<TSource, dynamic>> expression)
            => expression;

        public static Expression GetConcreteExpr<TSource, TResult>(Expression<Func<TSource, TResult>> expression)
            => expression;

        private class ParamItem
        {
            public string Name { get; set; }
            public string City { get; set; }
            public string Country { get; set; }
            public int Age { get; set; }
        }

        private class SourceItem
        {
            public string FullName { get; set; }
            public string City { get; set; }
            public int Age { get; set; }
        }
    }
}

