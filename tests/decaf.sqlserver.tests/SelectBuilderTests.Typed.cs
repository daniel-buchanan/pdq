using System;
using decaf.common;
using decaf.tests.common.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace decaf.sqlserver.tests
{
    public class SelectBuilderTypedTests : SqlServerTest
    {
        private readonly IQueryContainer query;

        public SelectBuilderTypedTests() : base()
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
            var expected = "select\\r\\n  u.Email as email,\\r\\n  u.Id as id\\r\\nfrom\\r\\n  User u\\r\\nwhere\\r\\n(u.Id = @p1)\\r\\n";
            expected = expected.Replace("\\r\\n", Environment.NewLine);
            var subValue = Guid.NewGuid();

            // Act
            var q = this.query.Select()
                .From<User>(u => u)
                .Where(u => u.Id == 42)
                .Select(u => new
                {
                    email = u.Email,
                    id = u.Id
                });

            var sql = q.GetSql();

            // Assert
            sql.Should().Be(expected);
        }

        [Fact]
        public void SimpleSelectReturnsCorrectParameters()
        {
            // Arrange
            var idValue = 42;

            // Act
            var q = this.query.Select()
                .From<User>(u => u)
                .Where(u => u.Id == idValue)
                .Select(u => new
                {
                    email = u.Email,
                    id = u.Id
                });

            var parameters = q.GetSqlParameters();

            // Assert
            parameters.Should().Satisfy(
                p => p.Key == "p1" && ((int)p.Value) == idValue);
        }

        [Fact]
        public void SimpleSelectWithOrderBySucceeds()
        {
            // Arrange
            var expected = "select\\r\\n  u.Email as email,\\r\\n  u.Id as id\\r\\nfrom\\r\\n  User u\\r\\nwhere\\r\\n(u.Id = @p1)\\r\\norder by\\r\\n  u.Id descending\\r\\n";
            expected = expected.Replace("\\r\\n", Environment.NewLine);
            var subValue = Guid.NewGuid();

            // Act
            var q = this.query.Select()
                .From<User>(u => u)
                .Where(u => u.Id == 42)
                .OrderBy(u => u.Id, SortOrder.Descending)
                .Select(u => new
                {
                    email = u.Email,
                    id = u.Id
                });

            var sql = q.GetSql();

            // Assert
            sql.Should().Be(expected);
        }

        [Fact]
        public void SelectWithLikeSucceeds()
        {
            // Arrange
            var expected = "select\\r\\n  u.Email as email,\\r\\n  u.Id as id\\r\\nfrom\\r\\n  User u\\r\\nwhere\\r\\n(u.Email like '%@p1%')\\r\\n";
            expected = expected.Replace("\\r\\n", Environment.NewLine);
            var subValue = Guid.NewGuid();

            // Act
            var q = this.query.Select()
                .From<User>(u => u)
                .Where(u => u.Email.Contains("bob"))
                .Select(u => new
                {
                    email = u.Email,
                    id = u.Id
                });

            var sql = q.GetSql();

            // Assert
            sql.Should().Be(expected);
        }

        [Fact]
        public void SelectWithStartsWithSucceeds()
        {
            // Arrange
            var expected = "select\\r\\n  u.Email as email,\\r\\n  u.Id as id\\r\\nfrom\\r\\n  User u\\r\\nwhere\\r\\n(u.Email like '@p1%')\\r\\n";
            expected = expected.Replace("\\r\\n", Environment.NewLine);
            var subValue = Guid.NewGuid();

            // Act
            var q = this.query.Select()
                .From<User>(u => u)
                .Where(u => u.Email.StartsWith("bob"))
                .Select(u => new
                {
                    email = u.Email,
                    id = u.Id
                });

            var sql = q.GetSql();

            // Assert
            sql.Should().Be(expected);
        }

        [Fact]
        public void SelectWithEndsWithSucceeds()
        {
            // Arrange
            var expected = "select\\r\\n  u.Email as email,\\r\\n  u.Id as id\\r\\nfrom\\r\\n  User u\\r\\nwhere\\r\\n(u.Email like '%@p1')\\r\\n";
            expected = expected.Replace("\\r\\n", Environment.NewLine);
            var subValue = Guid.NewGuid();

            // Act
            var q = this.query.Select()
                .From<User>(u => u)
                .Where(u => u.Email.EndsWith("bob"))
                .Select(u => new
                {
                    email = u.Email,
                    id = u.Id
                });

            var sql = q.GetSql();

            // Assert
            sql.Should().Be(expected);
        }

        [Fact]
        public void SelectWithMultipleConditionsSucceeds()
        {
            // Arrange
            var expected = "select\\r\\n  u.Email as email,\\r\\n  u.Id as id\\r\\nfrom\\r\\n  User u\\r\\nwhere\\r\\n(\\r\\n  (u.Email like '%@p1')\\r\\n  and\\r\\n  (u.FirstName = @p2)\\r\\n)\\r\\n";
            expected = expected.Replace("\\r\\n", Environment.NewLine);
            var subValue = Guid.NewGuid();

            // Act
            var q = this.query.Select()
                .From<User>(u => u)
                .Where(u => u.Email.EndsWith(".com") && u.FirstName == "Bob")
                .Select(u => new
                {
                    email = u.Email,
                    id = u.Id
                });

            var sql = q.GetSql();

            // Assert
            sql.Should().Be(expected);
        }

        [Fact]
        public void SelectWithManyConditionsSucceeds()
        {
            // Arrange
            var expected = "select\\r\\n  u.Email as email,\\r\\n  u.Id as id\\r\\nfrom\\r\\n  User u\\r\\nwhere\\r\\n(\\r\\n  (\\r\\n    (u.Id = @p1)\\r\\n    and\\r\\n    (u.Email like '%@p2')\\r\\n  )\\r\\n  and\\r\\n  (\\r\\n    (\\r\\n      not\\r\\n      (u.Id = @p3)\\r\\n    )\\r\\n    or\\r\\n    (u.Email like '%@p4')\\r\\n  )\\r\\n)\\r\\n";
            expected = expected.Replace("\\r\\n", Environment.NewLine);
            var subValue = Guid.NewGuid();

            // Act
            var q = this.query.Select()
                .From<User>(u => u)
                .Where(u => u.Id == 42 && u.Email.EndsWith(".com") && (u.Id != 0 || u.Email.EndsWith("abc")))
                .Select(u => new
                {
                    email = u.Email,
                    id = u.Id
                });

            var sql = q.GetSql();

            // Assert
            sql.Should().Be(expected);
        }

        [Fact]
        public void SelectWithSimpleJoinSucceeds()
        {
            // Arrange
            var expected = "select\\r\\n  p.FirstName as fname,\\r\\n  p.LastName as lname,\\r\\n  u.Email as email,\\r\\n  u.Id as id\\r\\nfrom\\r\\n  User u\\r\\njoin Person p on\\r\\n  (p.Id = u.PersonId)\\r\\nwhere\\r\\n(u.Id = @p1)\\r\\n";
            expected = expected.Replace("\\r\\n", Environment.NewLine);

            // Arrange
            var q = this.query.Select()
                .From<User>(u => u)
                .Join<Person>((u, p) => p.Id == u.PersonId)
                .Where((u, p) => u.Id == 42)
                .Select((u, p) => new
                {
                    fname = p.FirstName,
                    lname = p.LastName,
                    email = u.Email,
                    id = u.Id
                });

            var sql = q.GetSql();

            // Assert
            sql.Should().Be(expected);
        }

        [Fact]
        public void SelectFromQuerySucceeds()
        {
            // Arrange
            var expected = "select\n  q.Id as id,\n  q.FirstName as name\nfrom\n  (\n    select\n      u.Id as id,\n      u.FirstName as name\n    from\n      User u\n    where\n    (u.Id = @p1)\n  ) q\nwhere\n(q.FirstName like '%@p2')\n";
            expected = expected.Replace("\n", Environment.NewLine);

            // Arrange
            var q = this.query.Select()
                .From<User>(q =>
                {
                    q.From<User>(u => u)
                    .Where(u => u.Id == 42)
                    .Select(u => new
                    {
                        id = u.Id,
                        name = u.FirstName
                    });
                }, "q")
                .Where(q => q.FirstName.EndsWith("Smith"))
                .Select(q => new
                {
                    id = q.Id,
                    name = q.FirstName
                });

            var sql = q.GetSql();

            // Assert
            sql.Should().Be(expected);
        }
    }
}

