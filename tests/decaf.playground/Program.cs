// See https://aka.ms/new-console-template for more information

using System;
using Microsoft.Extensions.DependencyInjection;
using decaf;
using decaf.common;
using decaf.common.Connections;
using decaf.tests.common.Mocks;
using decaf.tests.common.Models;

var services = new ServiceCollection();
services.AddDecaf(o =>
{
    o.TrackUnitsOfWork()
        .OverrideDefaultLogLevel(LogLevel.Debug)
        .UseMockDatabase();
})
.WithConnection<IConnectionDetails, MockConnectionDetails>();

var provider = services.BuildServiceProvider();

Console.WriteLine("Hello, World!");

var decaf = provider.GetService<IDecaf>();
using (var q = decaf.Query())
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
        })
        .Execute();

    q.Select()
        .From<Person>(x => x)
        .Join<Address>((p, a) => p.AddressId == a.Id)
        .Where((p, a) => p.LastName.Contains("smith"))
        .Select((p, a) => new Result
        {
            Name = p.FirstName,
            City = a.City
        });

    q.Insert()
        .Into("bob")
        .Columns(b => new
        {
            id = b.Is<int>(),
            name = b.Is<string>()
        })
        .Value(new
        {
            id = 42,
            name = "smith"
        });

    q.Update()
        .Table<Person>()
        .From<Person>(b =>
        {
            b.KnownAs("x");
            b.From<Person>(p => p)
                .Where(p => p.CreatedAt < DateTime.Now)
                ;
        })
        .Set(x => x.Email, y => y.Email)
        .Where((x, y) => x.Id == y.Id)
        .Output(x => x.Id);

}
