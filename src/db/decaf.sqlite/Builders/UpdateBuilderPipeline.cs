using decaf.common.Utilities;
using decaf.db.common.Builders;
using decaf.state;

namespace decaf.sqlite.Builders
{
    public class UpdateBuilderPipeline : db.common.ANSISQL.UpdateBuilderPipeline
    {
        public UpdateBuilderPipeline(
            DecafOptions options,
            IHashProvider hashProvider,
            IWhereBuilder whereBuilder,
            IQuotedIdentifierBuilder quotedIdentifierBuilder,
            IBuilderPipeline<ISelectQueryContext> selectBuilder,
            IConstants constants)
            : base(options, hashProvider, whereBuilder, quotedIdentifierBuilder, selectBuilder, constants)
        {
        }
    }
}