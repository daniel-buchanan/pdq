using System.Linq.Expressions;
using pdq.common;
using pdq.state.Utilities;

namespace pdq.state
{
	public interface IQueryContextInternal : IQueryContext
	{
		/// <summary>
        /// The <see cref="IExpressionHelper"/> which provides helper methods for
        /// dealing with <see cref="System.Linq.Expression"/>.
        /// </summary>
		internal IExpressionHelper ExpressionHelper { get; }

        /// <summary>
        /// The <see cref="IDynamicExpressionHelper"/> which provides helper methods
        /// for dealing with <see cref="Expression"/> that
        /// involve dynamics
        /// </summary>
        internal IDynamicExpressionHelper DynamicExpressionHelper { get; }

		/// <summary>
        /// The <see cref="IReflectionHelper"/> which provides helper methods for
        /// dealing with actions which require reflection.
        /// </summary>
		internal IReflectionHelper ReflectionHelper { get; }

        /// <summary>
        /// The <see cref="IQueryParsers"/> that provide helper methods for dealing
        /// with joins, where clauses and value parsing.
        /// </summary>
        internal IQueryParsers Parsers { get; }

        /// <summary>
        /// The Alias Manager for the current query context.
        /// </summary>
        internal IAliasManager AliasManager { get; }

		/// <summary>
        /// Adds a <see cref="IQueryTarget"/> to this context.
        /// </summary>
        /// <param name="target">The <see cref="IQueryTarget"/> to add.</param>
		internal void AddQueryTarget(IQueryTarget target);

        /// <summary>
        /// Adds a <see cref="IQueryTarget"/> to this context based on the provided expression.
        /// </summary>
        /// <param name="expression">The expression which defines the target.</param>
        /// <returns>The <see cref="IQueryTarget"/> that has been added.</returns>
		internal IQueryTarget AddQueryTarget(Expression expression);

        /// <summary>
        /// Get a query target based off an expression.
        /// </summary>
        /// <param name="expression">The expression to use to find the query target.</param>
        /// <returns>A query target if found, otherwise null.</returns>
        internal IQueryTarget GetQueryTarget(Expression expression);

        /// <summary>
        /// Get a query target based off an alias.
        /// </summary>
        /// <param name="alias">The alias of the query target to find.</param>
        /// <returns>A query target if found, otherwise null.</returns>
        internal IQueryTarget GetQueryTarget(string alias);
    }

    internal interface IQueryParsers
    {
        internal IParser Join { get; }

        internal IParser Where { get; }

        internal IParser Value { get; }
    }
}

