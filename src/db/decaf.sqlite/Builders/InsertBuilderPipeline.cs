using decaf.common.Utilities;
using decaf.db.common;
using decaf.db.common.Builders;
using decaf.state;

namespace decaf.sqlite.Builders
{
    public class InsertBuilderPipeline : db.common.ANSISQL.InsertBuilderPipeline
    {
        public InsertBuilderPipeline(
            DecafOptions options,
            IHashProvider hashProvider,
            IWhereBuilder whereBuilder,
            IQuotedIdentifierBuilder quotedIdentifierBuilder,
            IValueParser valueParser,
            IBuilderPipeline<ISelectQueryContext> selectBuilder,
            IConstants constants)
            : base(options, hashProvider, whereBuilder, quotedIdentifierBuilder, valueParser, selectBuilder, constants)
        {
        }
    }
}