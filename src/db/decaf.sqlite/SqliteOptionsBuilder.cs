using decaf.db.common;

namespace decaf.sqlite;

public class SqliteOptionsBuilder : 
    SqlOptionsBuilder<SqliteOptions, ISqliteOptionsBuilder, ISqliteConnectionDetails>,
    ISqliteOptionsBuilder
{
    /// <inheritdoc/>
    public override ISqliteOptionsBuilder WithConnectionString(string connectionString)
    {
        ConfigureProperty(x => x.ConnectionDetails, new SqliteConnectionDetails(connectionString));
        return this;
    }

    /// <inheritdoc/>
    public ISqliteOptionsBuilder WithFilePath(string path)
    {
        ConfigureProperty(x => x.DatabasePath, path);
        return this;
    }

    /// <inheritdoc/>
    public ISqliteOptionsBuilder InMemory()
    {
        ConfigureProperty(x => x.InMemory, true);
        return this;
    }

    /// <inheritdoc/>
    public ISqliteOptionsBuilder WithVersion(decimal version)
    {
        ConfigureProperty(x => x.Version, version);
        return this;
    }

    public ISqliteOptionsBuilder CreateNewDatabase()
    {
        ConfigureProperty(x => x.CreateNew, true);
        return this;
    }
}