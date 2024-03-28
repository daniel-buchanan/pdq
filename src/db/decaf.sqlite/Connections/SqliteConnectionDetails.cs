using System.Text;
using System.Text.RegularExpressions;
using decaf.common.Connections;
using decaf.common.Exceptions;

namespace decaf.sqlite;

public class SqliteConnectionDetails : 
    ConnectionDetails, 
    ISqliteConnectionDetails
{
    public SqliteConnectionDetails() { }

    public SqliteConnectionDetails(string connectionString) : base(connectionString) 
        => ParseConnectionString(connectionString);

    public SqliteConnectionDetails(SqliteOptions options)
    {
        FullUri = options.DatabasePath;
        InMemory = options.InMemory;
        Version = options.Version;
    }
    
    /// <summary>
    /// Not Implemented as only called from base, and calling method is overridden.
    /// </summary>
    /// <param name="connectionString"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    protected override ConnectionStringParsingException ValidateConnectionString(string connectionString) 
        => throw new NotImplementedException();

    protected override string HostRegex => throw new NotImplementedException();
    protected override string PortRegex => throw new NotImplementedException();
    protected override string DatabaseRegex => throw new NotImplementedException();
    protected override int DefaultPort => throw new NotImplementedException();
    
    public string? FullUri { get; private set; }
    
    public bool InMemory { get; private set; }
    
    public bool CreateNew { get; private set; }

    public decimal Version { get; private set; }

    protected sealed override void ParseConnectionString(string input, Action<string> additionalParsing = null)
    {
        var regex = new Regex(@"Data Source=([a-zA-Z0-9_\-\.:/]+);Version=([0-9\.]+);(New=True);");
        var match = regex.Match(input);
        if (!match.Success)
            throw new ConnectionStringParsingException("Cannot parse Connection String: " + input);

        FullUri = match.Groups[1].Value;
        Version = decimal.Parse(match.Groups[2].Value);
        
        if (match.Groups[3].Value == "New=True")
            CreateNew = true;

        if (FullUri == ":memory:")
            InMemory = true;
    }

    protected override Task<string> ConstructConnectionStringAsync(CancellationToken cancellationToken = default)
    {
        if (InMemory)
        {
            FullUri = ":memory:";
            CreateNew = true;
        }

        var sb = new StringBuilder();
        sb.AppendFormat("Data Source={0};", FullUri);
        sb.AppendFormat("Version={0};", Version);
        if (CreateNew) sb.Append("New=True;");

        return Task.FromResult(sb.ToString());
    }
}