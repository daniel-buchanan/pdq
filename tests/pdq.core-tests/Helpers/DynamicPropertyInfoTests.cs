using System;
using System.Collections.Generic;
using FluentAssertions;
using pdq.core_tests.Models;
using pdq.state;
using pdq.state.Conditionals.ValueFunctions;
using pdq.state.Utilities;
using Xunit;

namespace pdq.core_tests.Helpers
{
    public class DynamicPropertyInfoTests
    {
        public DynamicPropertyInfoTests()
        {
        }

        [Fact]
        public void SetNameSucceeds()
        {
            // Arrange
            var info = DynamicPropertyInfo.Create();

            // Act
            info.SetName("bob");

            // Assert
            info.Name.Should().NotBeNullOrWhiteSpace();
            info.Name.Should().Be("bob");
        }

        [Fact]
        public void SetNewNameSucceeds()
        {
            // Arrange
            var info = DynamicPropertyInfo.Create();

            // Act
            info.SetNewName("bob");

            // Assert
            info.NewName.Should().NotBeNullOrWhiteSpace();
            info.NewName.Should().Be("bob");
        }

        [Fact]
        public void SetAliasSucceeds()
        {
            // Arrange
            var info = DynamicPropertyInfo.Create();

            // Act
            info.SetAlias("b");

            // Assert
            info.Alias.Should().NotBeNullOrWhiteSpace();
            info.Alias.Should().Be("b");
        }

        [Fact]
        public void SetTypeSucceeds()
        {
            // Arrange
            var info = DynamicPropertyInfo.Create();

            // Act
            info.SetType(typeof(Person));

            // Assert
            info.Type.Should().NotBeNull();
            info.Type.Should().Be(typeof(Person));
        }

        [Theory]
        [MemberData(nameof(ValidValues))]
        public void SetValueSucceeds(object value)
        {
            // Arrange
            var info = DynamicPropertyInfo.Create();

            // Act
            info.SetValue(value);

            // Assert
            info.Value.Should().Be(value);
        }

        [Theory]
        [MemberData(nameof(ValidFunctions))]
        public void SetFunctionSucceeds(IValueFunction function)
        {
            // Arrange
            var info = DynamicPropertyInfo.Create();

            // Act
            info.SetFunction(function);

            // Assert
            info.Function.Should().Be(function);
        }

        public static IEnumerable<object[]> ValidValues
        {
            get
            {
                yield return new object[] { null };
                yield return new object[] { 42 };
                yield return new object[] { true };
                yield return new object[] { false };
                yield return new object[] { 42D };
                yield return new object[] { 42M };
                yield return new object[] { 42F };
                yield return new object[] { "hello world" };
            }
        }

        public static IEnumerable<object[]> ValidFunctions
        {
            get
            {
                yield return new object[] { StringContains.Create("smith") };
                yield return new object[] { ToLower.Create() };
                yield return new object[] { ToUpper.Create() };
                yield return new object[] { Substring.Create(0) };
                yield return new object[] { Substring.Create(0, 9) };
            }
        }
    }
}

