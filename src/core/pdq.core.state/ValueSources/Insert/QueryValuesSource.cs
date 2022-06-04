﻿namespace pdq.state.ValueSources.Insert
{
	public class QueryValuesSource : IInsertSource
	{
		private QueryValuesSource(ISelectQueryContext context)
		{
			Query = context;
		}

		public ISelectQueryContext Query { get; private set; }

		public static QueryValuesSource Create(ISelectQueryContext context)
        {
			return new QueryValuesSource(context);
        }
	}
}
