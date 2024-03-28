using System;
using decaf.common;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace decaf.sqlserver.tests
{
    public class SelectBuilderTests : SqlServerTest
    {
        private readonly IQueryContainer query;

        public SelectBuilderTests() : base()
        {
            BuildServiceProvider();

            var decaf = provider.GetService<IDecaf>();
            var transient = decaf.BuildUnit();
            this.query = transient.Query() as IQueryContainer;
        }

        [Fact]
        public void SimpleSelectSucceeds()
        {
            // Arrange
            var expected = "select\\r\\n  u.email,\\r\\n  u.sub as id\\r\\nfrom\\r\\n  users u\\r\\nwhere\\r\\n(u.sub = @p1)\\r\\n";
            expected = expected.Replace("\\r\\n", Environment.NewLine);
            var subValue = Guid.NewGuid();

            // Act
            var q = this.query.Select()
                .From("users", "u")
                .Where(b =>
                {
                    b.Column("sub", "u").Is().EqualTo(subValue);
                })
                .Select(c => new
                {
                    email = c.Is("email", "u"),
                    id = c.Is("sub", "u")
                });

            var sql = q.GetSql();

            // Assert
            sql.Should().Be(expected);
        }

        [Fact]
        public void SimpleSelectReturnsCorrectParameters()
        {
            // Arrange
            var subValue = Guid.NewGuid();

            // Act
            var q = this.query.Select()
                .From("users", "u")
                .Where(b =>
                {
                    b.Column("sub", "u").Is().EqualTo(subValue);
                })
                .Select(c => new
                {
                    email = c.Is("email", "u"),
                    id = c.Is("sub", "u")
                });

            var parameters = q.GetSqlParameters();

            // Assert
            parameters.Should().Satisfy(
                p => p.Key == "p1" && ((Guid)p.Value) == subValue);
        }

        [Fact]
        public void SimpleSelectWithBetweenSucceeds()
        {
            // Arrange
            var expected = "select\\r\\n  u.email,\\r\\n  u.sub as id\\r\\nfrom\\r\\n  users u\\r\\nwhere\\r\\n(u.id between @p1 and @p2)\\r\\n";
            expected = expected.Replace("\\r\\n", Environment.NewLine);

            // Act
            var q = this.query.Select()
                .From("users", "u")
                .Where(b =>
                {
                    b.Column("id", "u").Is().Between(1, 4);
                })
                .Select(c => new
                {
                    email = c.Is("email", "u"),
                    id = c.Is("sub", "u")
                });

            var sql = q.GetSql();

            // Assert
            sql.Should().Be(expected);
        }

        [Fact]
        public void SimpleSelectWithBetweenDatesSucceeds()
        {
            // Arrange
            var expected = "select\\r\\n  u.email,\\r\\n  u.sub as id\\r\\nfrom\\r\\n  users u\\r\\nwhere\\r\\n(u.created_at between @p1 and @p2)\\r\\n";
            expected = expected.Replace("\\r\\n", Environment.NewLine);
            var start = DateTime.Now.AddDays(-1);
            var end = DateTime.Now;

            // Act
            var q = this.query.Select()
                .From("users", "u")
                .Where(b =>
                {
                    b.Column("created_at", "u").Is().Between(start, end);
                })
                .Select(c => new
                {
                    email = c.Is("email", "u"),
                    id = c.Is("sub", "u")
                });

            var sql = q.GetSql();

            // Assert
            sql.Should().Be(expected);
        }

        [Fact]
        public void SimpleSelectWithOrderBySucceeds()
        {
            // Arrange
            var expected = "select\\r\\n  u.email,\\r\\n  u.sub as id\\r\\nfrom\\r\\n  users u\\r\\nwhere\\r\\n(u.sub = @p1)\\r\\norder by\\r\\n  u.sub descending\\r\\n";
            expected = expected.Replace("\\r\\n", Environment.NewLine);
            var subValue = Guid.NewGuid();

            // Act
            var q = this.query.Select()
                .From("users", "u")
                .Where(b =>
                {
                    b.Column("sub", "u").Is().EqualTo(subValue);
                })
                .OrderBy("sub", "u", SortOrder.Descending)
                .Select(c => new
                {
                    email = c.Is("email", "u"),
                    id = c.Is("sub", "u")
                });

            var sql = q.GetSql();

            // Assert
            sql.Should().Be(expected);
        }

        [Fact]
        public void SelectWithLikeSucceeds()
        {
            // Arrange
            var expected = "select\\r\\n  u.email,\\r\\n  u.sub as id\\r\\nfrom\\r\\n  users u\\r\\nwhere\\r\\n(u.sub like '%@p1%')\\r\\n";
            expected = expected.Replace("\\r\\n", Environment.NewLine);
            var subValue = Guid.NewGuid();

            // Act
            var q = this.query.Select()
                .From("users", "u")
                .Where(b =>
                {
                    b.Column("sub", "u").Is().Like("bob");
                })
                .Select(c => new
                {
                    email = c.Is("email", "u"),
                    id = c.Is("sub", "u")
                });

            var sql = q.GetSql();

            // Assert
            sql.Should().Be(expected);
        }

        [Fact]
        public void SelectWithStartsWithSucceeds()
        {
            // Arrange
            var expected = "select\\r\\n  u.email,\\r\\n  u.sub as id\\r\\nfrom\\r\\n  users u\\r\\nwhere\\r\\n(u.sub like '@p1%')\\r\\n";
            expected = expected.Replace("\\r\\n", Environment.NewLine);
            var subValue = Guid.NewGuid();

            // Act
            var q = this.query.Select()
                .From("users", "u")
                .Where(b =>
                {
                    b.Column("sub", "u").Is().StartsWith("bob");
                })
                .Select(c => new
                {
                    email = c.Is("email", "u"),
                    id = c.Is("sub", "u")
                });

            var sql = q.GetSql();

            // Assert
            sql.Should().Be(expected);
        }

        [Fact]
        public void SelectWithEndsWithSucceeds()
        {
            // Arrange
            var expected = "select\\r\\n  u.email,\\r\\n  u.sub as id\\r\\nfrom\\r\\n  users u\\r\\nwhere\\r\\n(u.sub like '%@p1')\\r\\n";
            expected = expected.Replace("\\r\\n", Environment.NewLine);
            var subValue = Guid.NewGuid();

            // Act
            var q = this.query.Select()
                .From("users", "u")
                .Where(b =>
                {
                    b.Column("sub", "u").Is().EndsWith("bob");
                })
                .Select(c => new
                {
                    email = c.Is("email", "u"),
                    id = c.Is("sub", "u")
                });

            var sql = q.GetSql();

            // Assert
            sql.Should().Be(expected);
        }

        [Fact]
        public void SelectWithMultipleConditionsSucceeds()
        {
            // Arrange
            var expected = "select\\r\\n  u.email,\\r\\n  u.sub as id\\r\\nfrom\\r\\n  users u\\r\\nwhere\\r\\n(\\r\\n  (u.sub = @p1)\\r\\n  and\\r\\n  (u.email like '%@p2')\\r\\n)\\r\\n";
            expected = expected.Replace("\\r\\n", Environment.NewLine);
            var subValue = Guid.NewGuid();

            // Act
            var q = this.query.Select()
                .From("users", "u")
                .Where(b =>
                {
                    b.And(ba =>
                    {
                        ba.Column("sub", "u").Is().EqualTo("bob");
                        ba.Column("email", "u").Is().EndsWith(".com");
                    });
                })
                .Select(c => new
                {
                    email = c.Is("email", "u"),
                    id = c.Is("sub", "u")
                });

            var sql = q.GetSql();

            // Assert
            sql.Should().Be(expected);
        }

        [Fact]
        public void SelectWithManyConditionsSucceeds()
        {
            // Arrange
            var expected = "select\\r\\n  u.email,\\r\\n  u.sub as id\\r\\nfrom\\r\\n  users u\\r\\nwhere\\r\\n(\\r\\n  (u.sub = @p1)\\r\\n  and\\r\\n  (u.email like '%@p2')\\r\\n  and\\r\\n  (\\r\\n    (\\r\\n      not\\r\\n      (u.id = @p3)\\r\\n    )\\r\\n    or\\r\\n    (u.sub like '%@p4')\\r\\n  )\\r\\n)\\r\\n";
            expected = expected.Replace("\\r\\n", Environment.NewLine);
            var subValue = Guid.NewGuid();

            // Act
            var q = this.query.Select()
                .From("users", "u")
                .Where(b =>
                {
                    b.And(ba =>
                    {
                        ba.Column("sub", "u").Is().EqualTo("bob");
                        ba.Column("email", "u").Is().EndsWith(".com");
                        ba.Or(bo =>
                        {
                            bo.Column("id", "u").IsNot().EqualTo(0);
                            bo.Column("sub", "u").Is().EndsWith("abc");
                        });
                    });
                })
                .Select(c => new
                {
                    email = c.Is("email", "u"),
                    id = c.Is("sub", "u")
                });

            var sql = q.GetSql();

            // Assert
            sql.Should().Be(expected);
        }

        [Fact]
        public void SelectWithSimpleJoinSucceeds()
        {
            // Arrange
            var expected = "select\\r\\n  r.name as role,\\r\\n  u.email,\\r\\n  u.sub as id\\r\\nfrom\\r\\n  users u\\r\\njoin roles r on\\r\\n  (\\r\\n    (r.user_id = u.id)\\r\\n  )\\r\\nwhere\\r\\n(id = @p1)\\r\\n";
            expected = expected.Replace("\\r\\n", Environment.NewLine);

            // Arrange
            var q = this.query.Select()
                .From("users", "u")
                .Join().From("users", "u").To("roles", "r").On(c =>
                {
                    c.Column("user_id", "r").Is().EqualTo().Column("id", "u");
                })
                .Where(b =>
                {
                    b.Column("id").Is().EqualTo(42);
                })
                .Select(c => new
                {
                    role = c.Is("name", "r"),
                    email = c.Is("email", "u"),
                    id = c.Is("sub", "u")
                });

            var sql = q.GetSql();

            // Assert
            sql.Should().Be(expected);
        }

        [Fact]
        public void SelectFromQuerySucceeds()
        {
            // Arrange
            var expected = "select\\r\\n  q.name,\\r\\n  q.id\\r\\nfrom\\r\\n  (\\r\\n    select\\r\\n      u.id,\\r\\n      u.name\\r\\n    from\\r\\n      users u\\r\\n    where\\r\\n    (u.id = @p1)\\r\\n  ) q\\r\\nwhere\\r\\n(q.name like '%@p2')\\r\\n";
            expected = expected.Replace("\\r\\n", Environment.NewLine);

            // Arrange
            var q = this.query.Select()
                .From(q =>
                {
                    q.From("users", "u")
                     .Where(b => b.Column("id", "u").Is().EqualTo(42))
                     .Select(e => new
                     {
                         id = e.Is("id", "u"),
                         name = e.Is("name", "u")
                     });
                }, "q")
                .Where(b =>
                {
                    b.Column("name", "q").Is().EndsWith("Smith");
                })
                .Select(c => new
                {
                    name = c.Is("name", "q"),
                    id = c.Is("id", "q")
                });

            var sql = q.GetSql();

            // Assert
            sql.Should().Be(expected);
        }
    }
}

