using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using decaf.common;
using decaf.common.Connections;
using decaf.tests.common.Mocks;
using decaf.tests.common.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using decaf.services;
using Xunit;

namespace decaf.core_tests
{
    public class TransientExtensionsTests
    {
        private readonly IUnitOfWork unitOfWork;

        public TransientExtensionsTests()
        {
            var services = new ServiceCollection();
            services.AddDecaf(o =>
            {
                o.TrackUnitsOfWork();
                o.OverrideDefaultLogLevel(LogLevel.Debug);
                o.UseMockDatabase();
            });
            services.AddScoped<IConnectionDetails, MockConnectionDetails>();

            var provider = services.BuildServiceProvider();
            var decaf = provider.GetService<IDecaf>();
            this.unitOfWork = decaf.BuildUnit();
        }

        [Theory]
        [MemberData(nameof(ExtensionTests))]
        public void ServicesShouldNotBeNull<T>(Func<IUnitOfWork, T> method)
        {
            // Act
            var result = method(this.unitOfWork);

            // Assert
            result.Should().NotBeNull();
        }

        public static IEnumerable<object[]> ExtensionTests
        {
            get
            {
                yield return new object[] { GetMethod(t => t.GetCommand<Person>()) };
                yield return new object[] { GetMethod(t => t.GetQuery<Person>()) };
                yield return new object[] { GetMethod(t => t.GetService<Person>()) };

                yield return new object[] { GetMethod(t => t.GetCommand<Person, int>()) };
                yield return new object[] { GetMethod(t => t.GetQuery<Person, int>()) };
                yield return new object[] { GetMethod(t => t.GetService<Person, int>()) };

                yield return new object[] { GetMethod(t => t.GetCommand<Address, int, int>()) };
                yield return new object[] { GetMethod(t => t.GetQuery<Address, int, int>()) };
                yield return new object[] { GetMethod(t => t.GetService<Address, int, int>()) };

                yield return new object[] { GetMethod(t => t.GetCommand<AddressNote, int, int, int>()) };
                yield return new object[] { GetMethod(t => t.GetQuery<AddressNote, int, int, int>()) };
                yield return new object[] { GetMethod(t => t.GetService<AddressNote, int, int, int>()) };

            }
        }

        private static Func<IUnitOfWork, T> GetMethod<T>(Expression<Func<IUnitOfWork, T>> expression)
            => expression.Compile();
    }
}

