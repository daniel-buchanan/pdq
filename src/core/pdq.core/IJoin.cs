using System;
using pdq.common;
using pdq.state;

namespace pdq
{
	public interface IJoin
	{
		/// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="conditions"></param>
        /// <param name="to"></param>
        /// <param name="type"></param>
        /// <returns></returns>
		ISelectFrom Join(IQueryTarget from, state.IWhere conditions, IQueryTarget to, JoinType type = JoinType.Default);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="conditions"></param>
        /// <param name="query"></param>
        /// <param name="type"></param>
        /// <returns></returns>
		ISelectFrom Join(IQueryTarget from, state.IWhere conditions, Action<ISelectWithAlias> query, JoinType type = JoinType.Default);
	}
}
