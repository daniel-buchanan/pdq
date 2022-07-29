using System;
using System.Linq.Expressions;

namespace pdq
{
    public interface ISelectColumn
	{
        /// <summary>
        /// Select the result of the query as a dynamic object
        /// </summary>
        /// <param name="expression">The expression / lambda that defines the object structure returned.</param>
        /// <returns>The ability to execute the query, see <see cref="IExecuteDynamic"/> for more information.</returns>
        IExecuteDynamic Select(Expression<Func<ISelectColumnBuilder, dynamic>> expression);

        /// <summary>
        /// Select the result of the query as a strongly typed object
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="expression">The expression / lambda that defines the object returned.</param>
        /// <returns>The ability to execute the query, see <see cref="IExecute{T}"/> for more information.</returns>
        IExecute<TResult> Select<TResult>(Expression<Func<ISelectColumnBuilder, TResult>> expression);
	}
}

