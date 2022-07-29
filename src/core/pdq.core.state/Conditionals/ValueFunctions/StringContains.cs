using System;
using pdq.common;

namespace pdq.state.Conditionals.ValueFunctions
{
	public class StringContains : ValueFunction<string>
	{
		private StringContains(string value)
			: base(ValueFunction.Contains)
		{
			Value = value;
		}

		public string Value { get; private set; }

		public static StringContains Create(string value) => new StringContains(value);
	}
}

