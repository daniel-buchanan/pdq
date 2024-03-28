using System;
using decaf.common;
using decaf.tests.common.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace decaf.sqlserver.tests
{
    public class DeleteBuilderTypedTests : SqlServerTest
    {
        private readonly IQueryContainer query;

        public DeleteBuilderTypedTests() : base()
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
            var expected = "delete from\\r\\n  User u\\r\\nwhere\\r\\n(u.Subject = @p1)\\r\\noutput\\r\\n  Id\\r\\n";
            expected = expected.Replace("\\r\\n", Environment.NewLine);
            var subValue = Guid.NewGuid();

            // Act
            var q = this.query.Delete()
                .From<User>(u => u)
                .Where(u => u.Subject == subValue)
                .Output(u => u.Id);

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
                .From<User>(u => u)
                .Where(u => u.Subject == subValue);

            var parameters = q.GetSqlParameters();

            // Assert
            parameters.Should().Satisfy(
                p => p.Key == "p1" && ((Guid)p.Value) == subValue);
        }

        [Fact]
        public void DeleteWithLikeSucceeds()
        {
            // Arrange
            var expected = "delete from\\r\\n  User u\\r\\nwhere\\r\\n(u.FirstName like '%@p1%')\\r\\noutput\\r\\n  Id\\r\\n";
            expected = expected.Replace("\\r\\n", Environment.NewLine);
            var subValue = Guid.NewGuid();

            // Act
            var q = this.query.Delete()
                .From<User>(u => u)
                .Where(u => u.FirstName.Contains("bob"))
                .Output(u => u.Id);

            var sql = q.GetSql();

            // Assert
            sql.Should().Be(expected);
        }

        [Fact]
        public void DeleteWithMultipleConditionsSucceeds()
        {
            // Arrange
            var expected = "delete from\\r\\n  User u0\\r\\nwhere\\r\\n(\\r\\n  (u0.FirstName like '@p1%')\\r\\n  and\\r\\n  (u0.Email like '%@p2')\\r\\n)\\r\\noutput\\r\\n  Id,\\r\\n  Subject\\r\\n";
            expected = expected.Replace("\\r\\n", Environment.NewLine);
            var subValue = Guid.NewGuid();

            // Act
            var q = this.query.Delete()
                .From<User>()
                .Where(u => u.FirstName.StartsWith("bob") && u.Email.EndsWith(".com"))
                .Output(u => u.Id)
                .Output(u => u.Subject);

            var sql = q.GetSql();

            // Assert
            sql.Should().Be(expected);
        }
    }
}

