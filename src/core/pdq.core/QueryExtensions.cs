using pdq.common;
using pdq.state;

namespace pdq
{
	public static class QueryExtensions
	{
		/// <summary>
        /// Begin a delete query, this will allow you to remove records from a table,
        /// view or other schema objects within your database.
        /// </summary>
        /// <param name="query">The query instance that will become a "delete" query.</param>
        /// <returns>(FluentApi) The ability to specify the table, view or other schema item to delete from.</returns>
        /// <example>q.Delete().From("users", "u").Where(b => b.Column("name").Is().StartsWith("bob");</example>
		public static IDelete Delete(this IQuery query)
            => Implementation.Delete.Create(query.CreateContext<IDeleteQueryContext>(), query);

        /// <summary>
        /// Begin a select query, this will allow you to retrieve records from one or more
        /// tables, perform joins and other complex query semantics to produce the required output.
        /// </summary>
        /// <param name="query">The query instance that will become a "select" query.</param>
        /// <returns>(FluentApi) The ability to specify the base table, view or other schema item to select from.</returns>
        /// <example>q.Select().From("users", "u").Where(b => b.Column("name").Is().EndssWith("bob");</example>
        public static ISelect Select(this IQuery query)
            => Implementation.Select.Create(query.CreateContext<ISelectQueryContext>(), query);
	}
}

