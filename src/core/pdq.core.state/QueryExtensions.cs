using pdq.common;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("pdq")]
namespace pdq.state
{
	internal static class QueryExtensions
	{
		public static T CreateContext<T>(this IQuery query) where T: IQueryContext
        {
			var internalQuery = query as IQueryInternal;

			if (internalQuery == null) return default(T);

			var queryContextType = typeof(T);

			if(queryContextType.IsAssignableFrom(typeof(ISelectQueryContext)))
				return (T)SelectQueryContext.Create(internalQuery.AliasManager);

			if (queryContextType.IsAssignableFrom(typeof(IDeleteQueryContext)))
				return (T)DeleteQueryContext.Create(internalQuery.AliasManager);

			return default(T);
        }
	}
}

