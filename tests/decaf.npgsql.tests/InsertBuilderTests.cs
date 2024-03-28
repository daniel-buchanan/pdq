using System;
using decaf.common;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace decaf.npgsql.tests
{
    public class InsertBuilderTests : NpgsqlTest
    {
        private readonly IQueryContainer query;

        public InsertBuilderTests() : base()
        {
            BuildServiceProvider();

            var decaf = provider.GetService<IDecaf>();
            var transient = decaf.BuildUnit();
            this.query = transient.Query() as IQueryContainer;
        }

        [Fact]
        public void InsertSucceeds()
        {
            // Arrange
            var expected = "insert into\\r\\n  users\\r\\n  (first_name,last_name)\\r\\nvalues\\r\\n  (@p1,@p2)\\r\\n";
            expected = expected.Replace("\\r\\n", Environment.NewLine);

            // Act
            var q = this.query.Insert()
                .Into("users")
                .Columns(c => new
                {
                    first_name = c.Is<string>(),
                    last_name = c.Is<string>()
                })
                .Values(new[]
                {
                    new { first_name = "bob", last_name = "smith" }
                });

            var sql = q.GetSql();

            // Assert
            sql.Should().Be(expected);
        }

        [Fact]
        public void InsertWithOutputSucceeds()
        {
            // Arrange
            var expected = "insert into\\r\\n  users\\r\\n  (first_name,last_name)\\r\\nvalues\\r\\n  (@p1,@p2)\\r\\nreturning\\r\\n  id\\r\\n";
            expected = expected.Replace("\\r\\n", Environment.NewLine);

            // Act
            var q = this.query.Insert()
                .Into("users")
                .Columns(c => new
                {
                    first_name = c.Is<string>(),
                    last_name = c.Is<string>()
                })
                .Values(new[]
                {
                    new
                    {
                        first_name = "bob",
                        last_name = "smith"
                    }
                })
                .Output("id");

            var sql = q.GetSql();

            // Assert
            sql.Should().Be(expected);
        }

        [Fact]
        public void InsertMultipleValuesSucceeds()
        {
            // Arrange
            var expected = "insert into\\r\\n  users\\r\\n  (first_name,last_name)\\r\\nvalues\\r\\n  (@p1,@p2),\\r\\n  (@p3,@p2)\\r\\n";
            expected = expected.Replace("\\r\\n", Environment.NewLine);

            // Act
            var q = this.query.Insert()
                .Into("users")
                .Columns(c => new
                {
                    first_name = c.Is<string>(),
                    last_name = c.Is<string>()
                })
                .Values(new[]
                {
                    new { first_name = "bob", last_name = "smith" },
                    new { first_name = "james", last_name = "smith" }
                });

            var sql = q.GetSql();

            // Assert
            sql.Should().Be(expected);
        }

        [Fact]
        public void InsertFromQuerySucceeds()
        {
            // Arrange
            var expected = "insert into\\r\\n  users\\r\\n  (first_name,last_name)\\r\\nselect\\r\\n  t.first_name,\\r\\n  t.last_name\\r\\nfrom\\r\\n  temp as t\\r\\nwhere\\r\\n(t.id = @p1)\\r\\n";
            expected = expected.Replace("\\r\\n", Environment.NewLine);

            // Act
            var q = this.query.Insert()
                .Into("users")
                .Columns(c => new
                {
                    first_name = c.Is<string>(),
                    last_name = c.Is<string>()
                })
                .From(q =>
                {
                    q.From("temp", "t")
                        .Where(b => b.Column("id", "t").Is().EqualTo(42))
                        .Select(b => new
                        {
                            first_name = b.Is("first_name", "t"),
                            last_name = b.Is("last_name", "t")
                        });
                });

            var sql = q.GetSql();

            // Assert
            sql.Should().Be(expected);
        }

        [Fact]
        public void GetValuesSucceeds()
        {
            // Act
            var q = this.query.Insert()
                .Into("users")
                .Columns(c => new
                {
                    first_name = c.Is<string>(),
                    last_name = c.Is<string>()
                })
                .Values(new[]
                {
                    new { first_name = "bob", last_name = "smith" },
                    new { first_name = "james", last_name = "smith" }
                });

            var parameters = q.GetSqlParameters();

            // Assert
            parameters.Should().SatisfyRespectively(
                kp => kp.Key.Should().Be("p1"),
                kp => kp.Key.Should().Be("p2"),
                kp => kp.Key.Should().Be("p3"));
        }
    }
}

