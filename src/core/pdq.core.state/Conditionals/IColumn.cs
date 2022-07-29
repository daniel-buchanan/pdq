using System;
using pdq.common;

namespace pdq.state.Conditionals
{
	public interface IColumn : IWhere
	{
        Type ValueType { get; }

        state.Column Details { get; }

        EqualityOperator EqualityOperator { get; }

        IValueFunction ValueFunction { get; }

        object Value { get; }
    }
}

