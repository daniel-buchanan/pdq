using System;
using System.Collections.Generic;
using decaf.common.Connections;
using decaf.common.Logging;
using decaf.tests.common.Mocks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using Xunit;

namespace decaf.logging.tests;

public class DefaultLoggerTests
{
    private Mock<IStdOutputWrapper> output;
    private readonly IServiceProvider provider;
    
    public DefaultLoggerTests()
    {
        var services = new ServiceCollection();
        services.AddDecaf(o =>
            {
                o.TrackUnitsOfWork()
                    .OverrideDefaultLogLevel(LogLevel.Debug)
                    .UseMockDatabase();
            })
            .WithConnection<IConnectionDetails, MockConnectionDetails>();
        
        output = new Mock<IStdOutputWrapper>();
        services.Replace(new ServiceDescriptor(typeof(IStdOutputWrapper), output.Object));
        
        provider = services.BuildServiceProvider();
    }

    [Fact]
    public void StdOutputIsRegistered()
    {
        // Assert
        output.Should().NotBeNull();
    }

    [Fact]
    public void DefaultLoggerIsRegistered()
    {
        // Arrange
        var proxy = provider.GetService<ILoggerProxy>();

        // Assert
        proxy.Should().NotBeNull();
    }
    
    [Theory]
    [MemberData(nameof(LoggerOperations))]
    public void ProxyCallsLogger(Action<ILoggerProxy, string> method)
    {
        // Arrange
        var message = "hello world";
        var proxy = provider.GetRequiredService<ILoggerProxy>() as DefaultLoggerProxy;
        
        // Act
        method(proxy, message);

        // Assert
        output.Verify(l => l.WriteOut(It.IsAny<string>()));
    }

    [Fact]
    public void ProxyCallsError()
    {
        // Arrange
        var message = "hello world";
        var proxy = provider.GetRequiredService<ILoggerProxy>() as DefaultLoggerProxy;
        
        // Act
        proxy.Error(new Exception(message), message);
        
        // Assert
        output.Verify(l => l.WriteOut(It.IsAny<string>()));
    }

    public static IEnumerable<object[]> LoggerOperations
    {
        get
        {
            yield return new object[] { (ILoggerProxy p, string m) => p.Debug(m) };
            yield return new object[] { (ILoggerProxy p, string m) => p.Information(m) };
            yield return new object[] { (ILoggerProxy p, string m) => p.Warning(m) };
            yield return new object[] { (ILoggerProxy p, string m) => p.Error(m) };
        }
    }
}