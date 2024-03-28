using System;
using System.Collections.Generic;
using decaf.common.Connections;
using decaf.common.Logging;
using decaf.tests.common.Mocks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using decaf.logging.serilog;
using Serilog;
using Xunit;
using LoggerProxy = decaf.logging.serilog.LoggerProxy;
using serilog_LoggerProxy = decaf.logging.serilog.LoggerProxy;

namespace decaf.logging.tests;

public class SerilogTests
{
    private readonly Mock<ILogger> logger;
    private readonly IServiceProvider provider;
    
    public SerilogTests()
    {
        var services = new ServiceCollection();
        services.AddDecaf(o =>
            {
                o.TrackUnitsOfWork()
                    .OverrideDefaultLogLevel(LogLevel.Debug)
                    .UseMockDatabase()
                    .UseSerilog();
            })
            .WithConnection<IConnectionDetails, MockConnectionDetails>();

        logger = new Mock<ILogger>();
        services.AddScoped<ILogger>(p => logger.Object);
        provider = services.BuildServiceProvider();
    }
    
    [Fact]
    public void LoggerProxyIsRegistered()
    {
        // Act
        var proxy = provider.GetRequiredService<ILoggerProxy>() as serilog_LoggerProxy;

        // Assert
        proxy.Should().NotBeNull();
    }
    
    [Theory]
    [MemberData(nameof(LoggerOperations))]
    public void LoggerProxyCallsLogger(Action<ILoggerProxy, string> method)
    {
        // Arrange
        var message = "hello world";
        var proxy = provider.GetRequiredService<ILoggerProxy>() as serilog_LoggerProxy;
        
        // Act
        method(proxy, message);

        // Assert
        logger.VerifyAll();
    }

    [Fact]
    public void LoggerProxyCallsError()
    {
        // Arrange
        var message = "hello world";
        var proxy = provider.GetRequiredService<ILoggerProxy>() as serilog_LoggerProxy;
        
        // Act
        proxy.Error(new Exception(message), message);
        
        // Assert
        logger.Verify(l => l.Error(It.IsAny<Exception>(), It.IsAny<string>()));
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