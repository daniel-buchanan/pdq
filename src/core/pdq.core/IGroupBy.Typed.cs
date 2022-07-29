using System;
using System.Linq.Expressions;

namespace pdq
{
	public interface IGroupByTyped<T> :
        ISelectColumnTyped<T>,
        IOrderByTyped<T>
    {
		IGroupByThenTyped<T> GroupBy(Expression<Func<T, object>> builder);
	}

    public interface IGroupByTyped<T1, T2> :
        ISelectColumnTyped<T1, T2>,
        IOrderByTyped<T1, T2>
    {
        IGroupByThenTyped<T1, T2> GroupBy(Expression<Func<T1, T2, object>> builder);
    }

    public interface IGroupByTyped<T1, T2, T3> :
        ISelectColumnTyped<T1, T2, T3>,
        IOrderByTyped<T1, T2, T3>
    {
        IGroupByThenTyped<T1, T2, T3> GroupBy(Expression<Func<T1, T2, T3, object>> builder);
    }

    public interface IGroupByTyped<T1, T2, T3, T4> :
        ISelectColumnTyped<T1, T2, T3, T4>,
        IOrderByTyped<T1, T2, T3, T4>
    {
        IGroupByThenTyped<T1, T2, T3, T4> GroupBy(Expression<Func<T1, T2, T3, T4, object>> builder);
    }

    public interface IGroupByTyped<T1, T2, T3, T4, T5> :
        ISelectColumnTyped<T1, T2, T3, T4, T5>,
        IOrderByTyped<T1, T2, T3, T4, T5>
    {
        IGroupByThenTyped<T1, T2, T3, T4, T5> GroupBy(Expression<Func<T1, T2, T3, T4, T5, object>> builder);
    }

    public interface IGroupByThenTyped<T> :
        IOrderByTyped<T>,
        ISelectColumnTyped<T>
    {
		IGroupByThenTyped<T> ThenBy(Expression<Func<T, object>> builder);
	}

    public interface IGroupByThenTyped<T1, T2> :
        IOrderByTyped<T1, T2>,
        ISelectColumnTyped<T1, T2>
    {
        IGroupByThenTyped<T1, T2> ThenBy(Expression<Func<T1, T2, object>> builder);
    }

    public interface IGroupByThenTyped<T1, T2, T3> :
        IOrderByTyped<T1, T2, T3>,
        ISelectColumnTyped<T1, T2, T3>
    {
        IGroupByThenTyped<T1, T2, T3> ThenBy(Expression<Func<T1, T2, T3, object>> builder);
    }

    public interface IGroupByThenTyped<T1, T2, T3, T4> :
        IOrderByTyped<T1, T2, T3, T4>,
        ISelectColumnTyped<T1, T2, T3, T4>
    {
        IGroupByThenTyped<T1, T2, T3, T4> ThenBy(Expression<Func<T1, T2, T3, T4, object>> builder);
    }

    public interface IGroupByThenTyped<T1, T2, T3, T4, T5> :
        IOrderByTyped<T1, T2, T3, T4, T5>,
        ISelectColumnTyped<T1, T2, T3, T4, T5>
    {
        IGroupByThenTyped<T1, T2, T3, T4, T5> ThenBy(Expression<Func<T1, T2, T3, T4, T5, object>> builder);
    }
}

