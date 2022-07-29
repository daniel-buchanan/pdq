using System;
using FluentAssertions;
using pdq.Implementation;
using Xunit;

namespace pdq.core_tests
{
	public class SelecColumnBuilderTests
	{
        private readonly ISelectColumnBuilder builder;

		public SelecColumnBuilderTests()
		{
			this.builder = new SelectColumnBuilder();
		}

        [Fact]
		public void IsReturnsNull()
        {
			var value = this.builder.Is("name");
			value.Should().BeNull();
        }

        [Fact]
        public void IsWithAliasReturnsNull()
        {
            var value = this.builder.Is("name", "p");
            value.Should().BeNull();
        }

        [Fact]
        public void IsGenericReturnsNull()
        {
            var value = this.builder.Is<string>("name");
            value.Should().BeNull();
        }

        [Fact]
        public void IsGenericWithAliasReturnsNull()
        {
            var value = this.builder.Is<string>("name", "p");
            value.Should().BeNull();
        }
    }
}

