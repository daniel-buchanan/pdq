// See https://aka.ms/new-console-template for more information

using System;
using Microsoft.Extensions.DependencyInjection;
using pdq;
using pdq.common;
using pdq.core_tests.Mocks;
using pdq.core_tests.Models;

var services = new ServiceCollection();
services.AddPdq(o =>
{
    o.EnableTransientTracking();
    o.OverrideDefaultLogLevel(LogLevel.Debug);
    o.UseMockDatabase();
});
services.AddScoped<IConnectionDetails, MockConnectionDetails>();

var provider = services.BuildServiceProvider();

Console.WriteLine("Hello, World!");

var uow = provider.GetService<IUnitOfWork>();
using (var t = uow.Begin())
{
    using (var q = t.Query())
    {
        q.Select()
            .From("bob", "b")
            .Where(b =>
            {
                b.ClauseHandling.DefaultToOr();

                b.Column("name").Is().EqualTo("hello");
                b.Column("email").Is().Like("my name");
            })
            .Select(b => new
            {
                Name = b.Is("name", "b"),
                City = b.Is("city", "b")
            });

        q.Select()
            .From<Person>(x => x)
            .Join<Address>((p, a) => p.AddressId == a.Id)
            .Where((p, a) => p.LastName.Contains("smith"))
            .Select((p, a) => new Result
            {
                Name = p.FirstName,
                City = a.City
            });

    }
    
}
