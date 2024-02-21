﻿using pdq.common.Utilities;
using pdq.db.common;
using pdq.db.common.Builders;
using pdq.state;

namespace pdq.npgsql.Builders
{
	public class InsertBuilderPipeline : db.common.ANSISQL.InsertBuilderPipeline
	{
        public InsertBuilderPipeline(
            PdqOptions options,
            IHashProvider hashProvider,
            db.common.Builders.IWhereBuilder whereBuilder,
            IQuotedIdentifierBuilder quotedIdentifierBuilder,
            IValueParser valueParser,
            IBuilderPipeline<ISelectQueryContext> selectBuilder,
            IConstants constants)
            : base(options, hashProvider, whereBuilder, quotedIdentifierBuilder, valueParser, selectBuilder, constants)
        {
        }
    }
}

