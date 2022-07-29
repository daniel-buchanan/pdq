using System;
using System.Linq.Expressions;
using pdq.common;
using pdq.state;

namespace pdq
{
	public interface IJoinTyped<T>
	{
        ISelectFromTyped<T, TDestination> Join<TDestination>(Expression<Func<T, TDestination, bool>> joinExpression, JoinType type = JoinType.Default);
        ISelectFromTyped<T, TDestination> Join<TDestination>(Action<ISelectWithAlias> query, Expression<Func<T, TDestination, bool>> joinExpression, JoinType type = JoinType.Default);
        ISelectFromTyped<T, TDestination> Join<TDestination>(Func<ISelectWithAlias, Expression<Func<T, TDestination, bool>>> query, JoinType type = JoinType.Default);
    }

    public interface IJoinTyped<T1, T2>
    {
        ISelectFromTyped<T1, T2, TDestination> Join<T, TDestination>(Expression<Func<T, TDestination, bool>> joinExpression, JoinType type = JoinType.Default)
            where T : T1;
        ISelectFromTyped<T1, T2, TDestination> Join<TDestination>(Expression<Func<T2, TDestination, bool>> joinExpression, JoinType type = JoinType.Default);
        ISelectFromTyped<T1, T2, TDestination> Join<TDestination>(Action<ISelectWithAlias> query, Expression<Func<T2, TDestination, bool>> joinExpression, JoinType type = JoinType.Default);
    }

    public interface IJoinTyped<T1, T2, T3> 
    {
        ISelectFromTyped<T1, T2, T3, TDestination> Join<T, TDestination>(Expression<Func<T, TDestination, bool>> joinExpression, JoinType type = JoinType.Default)
            where T : T1;
        ISelectFromTyped<T1, T2, T3, TDestination> Join<TDestination>(Expression<Func<T3, TDestination, bool>> joinExpression, JoinType type = JoinType.Default);
        ISelectFromTyped<T1, T2, T3, TDestination> Join<TDestination>(Action<ISelectWithAlias> query, Expression<Func<T3, TDestination, bool>> joinExpression, JoinType type = JoinType.Default);
    }

    public interface IJoinTyped<T1, T2, T3, T4>
    {
        ISelectFromTyped<T1, T2, T3, T4, TDestination> Join<T, TDestination>(Expression<Func<T, TDestination, bool>> joinExpression, JoinType type = JoinType.Default)
            where T : T1;
        ISelectFromTyped<T1, T2, T3, T4, TDestination> Join<TDestination>(Expression<Func<T4, TDestination, bool>> joinExpression, JoinType type = JoinType.Default);
        ISelectFromTyped<T1, T2, T3, T4, TDestination> Join<TDestination>(Action<ISelectWithAlias> query, Expression<Func<T4, TDestination, bool>> joinExpression, JoinType type = JoinType.Default);
    }
}
