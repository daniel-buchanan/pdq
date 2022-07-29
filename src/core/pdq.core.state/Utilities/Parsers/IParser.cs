using System;
using System.Linq.Expressions;

namespace pdq.state.Utilities
{
	internal interface IParser
	{
		/// <summary>
        /// Parse a provided expression into a where clause.
        /// </summary>
        /// <param name="expression">The expression to parse.</param>
        /// <param name="context">The current query context</param>
        /// <returns>The parsed where clause.</returns>
		public IWhere Parse(Expression expression, IQueryContextInternal context);
	}
}

