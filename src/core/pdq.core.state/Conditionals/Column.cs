using System;
using pdq.common;

namespace pdq.state.Conditionals
{
	public static class Column
	{
        public static ColumnMatch Equals(state.Column left, EqualityOperator op, state.Column right)
            => new ColumnMatch(left, op, right);

        public static ColumnMatch Equals(state.Column left, EqualityOperator op, IValueFunction valueFunction, state.Column right)
            => new ColumnMatch(left, op, valueFunction, right);

        public static Column<T> Equals<T>(state.Column column, EqualityOperator op, T value, IValueFunction valueFunction = null)
            => new Column<T>(column, op, valueFunction, value);

        public static Column<T> Equals<T>(state.Column column, T value, IValueFunction valueFunction = null)
            => new Column<T>(column, EqualityOperator.Equals, valueFunction, value);

        public static Column<T> NotEquals<T>(state.Column column, T value, IValueFunction valueFunction = null)
            => new Column<T>(column, EqualityOperator.NotEquals, valueFunction, value);

        public static Column<T> LessThan<T>(state.Column column, T value, IValueFunction valueFunction = null)
            => new Column<T>(column, EqualityOperator.LessThan, valueFunction, value);

        public static Column<T> LessThanOrEqualTo<T>(state.Column column, T value, IValueFunction valueFunction = null)
            => new Column<T>(column, EqualityOperator.LessThanOrEqualTo, valueFunction, value);

        public static Column<T> GreaterThan<T>(state.Column column, T value, IValueFunction valueFunction = null)
            => new Column<T>(column, EqualityOperator.GreaterThan, valueFunction, value);

        public static Column<T> GreaterThanOrEqualTo<T>(state.Column column, T value, IValueFunction valueFunction = null)
            => new Column<T>(column, EqualityOperator.GreaterThanOrEqualTo, valueFunction, value);

        public static Column<T> Like<T>(state.Column column, T value, IValueFunction valueFunction = null)
            => new Column<T>(column, EqualityOperator.Like, valueFunction, value);

        public static Column<T> NotLike<T>(state.Column column, T value, IValueFunction valueFunction = null)
            => new Column<T>(column, EqualityOperator.NotLike, valueFunction, value);
    }
}

