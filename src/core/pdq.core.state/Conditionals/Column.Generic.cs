using System;
using pdq.common;

namespace pdq.state.Conditionals
{
    public class Column<T> : Where, IColumn
    {
		internal Column(
			state.Column column,
			EqualityOperator op,
			T value)
			: base()
		{
			ValueFunction = null;
			Details = column;
			EqualityOperator = op;
			Value = value;
			ValueType = typeof(T);
		}

		internal Column(
			state.Column column,
			EqualityOperator op,
			IValueFunction valueFunction,
			T value)
			: base()
		{
			ValueFunction = valueFunction;
			Details = column;
			EqualityOperator = op;
			Value = value;
			ValueType = typeof(T);
		}

		public Type ValueType { get; private set; }

		public state.Column Details { get; private set; }

		public EqualityOperator EqualityOperator { get; private set; }

		public IValueFunction ValueFunction { get; private set; }

		public T Value { get; private set; }

        object IColumn.Value => Value;
    }
}