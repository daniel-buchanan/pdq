using System;
namespace pdq.state.Conditionals
{
	public interface IBetween : IWhere
	{
        state.Column Column { get; }

        object Start { get; }

        object End { get; }

        Type ValueType { get; }
    }
}

