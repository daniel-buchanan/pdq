using System;
using decaf.common;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace decaf.npgsql.tests
{
    public class DeleteBuilderTests : NpgsqlTest
    {
        private readonly IQueryContainer query;

        public DeleteBuilderTests() : base()
        {
            BuildServiceProvider();

            var decaf = provider.GetService<IDecaf>();
            var transient = decaf.BuildUnit();
            this.query = transient.Query() as IQueryContainer;
        }

        [Fact]
        public void SimpleDeleteSucceeds()
        {
            // Arrange
            var expected = "delete from\\r\\n  users as u\\r\\nwhere\\r\\n(u.sub = @p1)\\r\\nreturning\\r\\n  id\\r\\n";
            expected = expected.Replace("\\r\\n", Environment.NewLine);
            var subValue = Guid.NewGuid();

            // Act
            var q = this.query.Delete()
                .From("users", "u")
                .Where(b => b.Column("sub", "u").Is().EqualTo(subValue))
                .Output("id");

            var sql = q.GetSql();

            // Assert
            sql.Should().Be(expected);
        }

        [Fact]
        public void SimpleDeleteReturnsCorrectParameters()
        {
            // Arrange
            var subValue = Guid.NewGuid();

            // Act
            var q = this.query.Delete()
                .From("users", "u")
                .Where(b => b.Column("sub", "u").Is().EqualTo(subValue));

            var parameters = q.GetSqlParameters();

            // Assert
            parameters.Should().Satisfy(
                p => p.Key == "p1" && ((Guid)p.Value) == subValue);
        }

        [Fact]
        public void DeleteWithLikeSucceeds()
        {
            // Arrange
            var expected = "delete from\\r\\n  users as u\\r\\nwhere\\r\\n(u.sub like '%@p1%')\\r\\nreturning\\r\\n  id\\r\\n";
            expected = expected.Replace("\\r\\n", Environment.NewLine);
            var subValue = Guid.NewGuid();

            // Act
            var q = this.query.Delete()
                .From("users", "u")
                .Where(b => b.Column("sub", "u").Is().Like("bob"))
                .Output("id");

            var sql = q.GetSql();

            // Assert
            sql.Should().Be(expected);
        }

        [Fact]
        public void DeleteWithStartsWithSucceeds()
        {
            // Arrange
            var expected = "delete from\\r\\n  users as u\\r\\nwhere\\r\\n(u.sub like '@p1%')\\r\\nreturning\\r\\n  id\\r\\n";
            expected = expected.Replace("\\r\\n", Environment.NewLine);
            var subValue = Guid.NewGuid();

            // Act
            var q = this.query.Delete()
                .From("users", "u")
                .Where(b => b.Column("sub", "u").Is().StartsWith("bob"))
                .Output("id");

            var sql = q.GetSql();

            // Assert
            sql.Should().Be(expected);
        }

        [Fact]
        public void DeleteWithEndsWithSucceeds()
        {
            // Arrange
            var expected = "delete from\\r\\n  users as u\\r\\nwhere\\r\\n(u.sub like '%@p1')\\r\\nreturning\\r\\n  id\\r\\n";
            expected = expected.Replace("\\r\\n", Environment.NewLine);
            var subValue = Guid.NewGuid();

            // Act
            var q = this.query.Delete()
                .From("users", "u")
                .Where(b => b.Column("sub", "u").Is().EndsWith("bob"))
                .Output("id");

            var sql = q.GetSql();

            // Assert
            sql.Should().Be(expected);
        }

        [Fact]
        public void DeleteWithMultipleConditionsSucceeds()
        {
            // Arrange
            var expected = "delete from\\r\\n  users as u\\r\\nwhere\\r\\n(\\r\\n  (u.sub = @p1)\\r\\n  and\\r\\n  (u.email like '%@p2')\\r\\n)\\r\\nreturning\\r\\n  id,\\r\\n  sub\\r\\n";
            expected = expected.Replace("\\r\\n", Environment.NewLine);
            var subValue = Guid.NewGuid();

            // Act
            var q = this.query.Delete()
                .From("users", "u")
                .Where(b =>
                {
                    b.And(ba =>
                    {
                        ba.Column("sub", "u").Is().EqualTo("bob");
                        ba.Column("email", "u").Is().EndsWith(".com");
                    });
                })
                .Output("id")
                .Output("sub");

            var sql = q.GetSql();

            // Assert
            sql.Should().Be(expected);
        }
    }
}

