using System;
using decaf.common;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace decaf.sqlserver.tests
{
    public class UpdateBuilderTests : SqlServerTest
    {
        private readonly IQueryContainer query;

        public UpdateBuilderTests() : base()
        {
            BuildServiceProvider();

            var decaf = provider.GetService<IDecaf>();
            var transient = decaf.BuildUnit();
            this.query = transient.Query() as IQueryContainer;
        }

        [Fact]
        public void SimpleUpdateSucceeds()
        {
            // Arrange
            var expected = "update\\r\\n  users\\r\\nset\\r\\n  sub = @p1\\r\\nwhere\\r\\n(\\r\\n  (id = @p2)\\r\\n)\\r\\n";
            expected = expected.Replace("\\r\\n", Environment.NewLine);
            var idValue = Guid.NewGuid();

            // Act
            var q = this.query.Update()
                .Table("users")
                .Set(new
                {
                    sub = "abc123"
                })
                .Where(b => b.Column("id").Is().EqualTo(idValue));

            var sql = q.GetSql();

            // Assert
            sql.Should().Be(expected);
        }

        [Fact]
        public void MultipleUpdateSucceeds()
        {
            // Arrange
            var expected = "update\\r\\n  users\\r\\nset\\r\\n  sub = @p1,\\r\\n  name = @p2\\r\\nwhere\\r\\n(\\r\\n  (id = @p3)\\r\\n)\\r\\n";
            expected = expected.Replace("\\r\\n", Environment.NewLine);
            var idValue = Guid.NewGuid();

            // Act
            var q = this.query.Update()
                .Table("users")
                .Set(new
                {
                    sub = "abc123",
                    name = "bob"
                })
                .Where(b => b.Column("id").Is().EqualTo(idValue));

            var sql = q.GetSql();

            // Assert
            sql.Should().Be(expected);
        }

        [Fact]
        public void UpdateFromSucceeds()
        {
            // Arrange
            var updatedDate = DateTime.UtcNow;
            var expected = "update\\r\\n  users\\r\\nset\\r\\n  name = x.new_name\\r\\nfrom\\r\\n(\\r\\n  select\\r\\n    tu.new_name\\r\\n  from\\r\\n    temp_users tu\\r\\n  where\\r\\n  (tu.id = id)\\r\\n) x";
            expected = expected.Replace("\\r\\n", Environment.NewLine);
            var idValue = Guid.NewGuid();

            // Act
            var q = this.query.Update()
                .Table("users")
                .From(sq =>
                {
                    sq.From("temp_users", "tu")
                        .Where(w => w.Column("id", "tu").Is().EqualTo().Column("id"))
                        .Select(b => new
                        {
                            new_name = b.Is("new_name", "tu")
                        });
                })
                .Set("name", "new_name");

            var sql = q.GetSql();

            // Assert
            sql.Should().Be(expected);
        }
    }
}

