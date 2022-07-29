using System;
using pdq.common;

namespace pdq.state.Conditionals
{
	public class ColumnMatch : Where
	{
		internal ColumnMatch(state.Column left, EqualityOperator op, state.Column right)
			: base()
		{
			Left = left;
			EqualityOperator = op;
			Right = right;
		}

		internal ColumnMatch(state.Column left, EqualityOperator op, IValueFunction valueFunction, state.Column right)
			: this(left, op, right)
        {
			RightFunction = valueFunction;
        }

		public state.Column Left { get; private set; }

		public EqualityOperator EqualityOperator { get; private set; }

		public IValueFunction RightFunction { get; private set; }

		public state.Column Right { get; private set; }        
	}
}

