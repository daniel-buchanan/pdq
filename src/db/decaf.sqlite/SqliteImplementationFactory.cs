using decaf.db.common;

namespace decaf.sqlite;

public class SqliteImplementationFactory : DbImplementationFactory
{
    /// <inheritdoc/>
    protected override Type ConnectionDetails()
        => typeof(SqliteConnectionDetails);

    /// <inheritdoc/>
    protected override Type ConnectionFactory()
        => typeof(SqliteConnectionFactory);

    /// <inheritdoc/>
    protected override Type TransactionFactory()
        => typeof(SqliteTransactionFactory);

    /// <inheritdoc/>
    protected override Type ValueParser()
        => typeof(SqliteValueParser);

    /// <inheritdoc/>
    protected override Type WhereBuilder()
        => typeof(Builders.WhereBuilder);

    /// <inheritdoc/>
    protected override Type SelectPipeline()
        => typeof(Builders.SelectBuilderPipeline);

    /// <inheritdoc/>
    protected override Type DeletePipeline()
        => typeof(Builders.DeleteBuilderPipeline);

    /// <inheritdoc/>
    protected override Type InsertPipeline()
        => typeof(Builders.InsertBuilderPipeline);

    /// <inheritdoc/>
    protected override Type UpdatePipeline()
        => typeof(Builders.UpdateBuilderPipeline);
}