using decaf.common.Options;
using decaf.db.common;

namespace decaf.sqlite;

public static class DecafOptionsBuilderExtensions
{
    /// <summary>
    /// Use SQLite (System.Data.Sqlite as the client) with the default configuration and options.
    /// </summary>
    /// <param name="builder">The <see cref="IDecafOptionsBuilder"/> to use.</param>
    public static IDecafOptionsBuilder UseSqlite(this IDecafOptionsBuilder builder)
        => UseSqlite(builder, new SqliteOptions());

    /// <summary>
    /// Use SQLite (System.Data.Sqlite as the client) with a builder for the options.
    /// </summary>
    /// <param name="optionsBuilder">The <see cref="IDecafOptionsBuilder"/> to use.</param>
    /// <param name="builder">An <see cref="Action{T}"/> to configure the <see cref="SqliteOptions"/>.</param>
    public static IDecafOptionsBuilder UseSqlite(
        this IDecafOptionsBuilder optionsBuilder,
        Action<ISqliteOptionsBuilder> builder)
    {
        var options = new SqliteOptionsBuilder();
        builder(options);
        return UseSqlite(optionsBuilder, options.Build());
    }

    /// <summary>
    /// Use SQLite (System.Data.Sqlite as the client) with a provided <see cref="SqliteOptions"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IDecafOptionsBuilder"/> to use.</param>
    /// <param name="options">The <see cref="SqliteOptions"/> to use.</param>
    public static IDecafOptionsBuilder UseSqlite(
        this IDecafOptionsBuilder builder,
        SqliteOptions options)
    {
        var x = builder as IDecafOptionsBuilderExtensions;
        x.UseDbImplementation<SqliteImplementationFactory>(options);
        return builder;
    }
}