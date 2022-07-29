using System;
using System.Linq.Expressions;

namespace pdq
{
	public interface ISelect : ISelectColumn
	{
		/// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <param name="alias"></param>
        /// <param name="schema"></param>
        /// <returns></returns>
		ISelectFrom From(string table, string alias, string schema = null);

		/// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="alias"></param>
        /// <returns></returns>
		ISelectFrom From(Action<ISelect> query, string alias);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		ISelectFromTyped<T> From<T>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aliasExpression">An expression which defines the alias for the table.</param>
        /// <returns></returns>
		ISelectFromTyped<T> From<T>(Expression<Func<T, T>> aliasExpression);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="alias"></param>
        /// <returns></returns>
        ISelectFromTyped<T> From<T>(Action<ISelectWithAlias> query, string alias);
    }

	public interface ISelectWithAlias : ISelect
    {
        /// <summary>
        /// 
        /// </summary>
		string Alias { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
		ISelectWithAlias KnownAs(string alias);
    }
}

