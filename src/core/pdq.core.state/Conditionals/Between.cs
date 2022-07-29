using System;

namespace pdq.state.Conditionals
{
	public abstract class Between : Where
	{
		public abstract Type ValueType { get; protected set; }

		public static Between<T> Values<T>(state.Column column, T start, T end)
        {
			return new Between<T>(column, start, end);
        }
	}

	public class Between<T> : Between, IBetween
	{
		internal Between(state.Column column, T start, T end)
		{
			Column = column;
			Start = start;
			End = end;
			ValueType = typeof(T);
		}

		public state.Column Column { get; private set; }

		public T Start { get; private set; }

		public T End { get; private set; }

        public override Type ValueType { get; protected set; }

        object IBetween.Start => Start;

        object IBetween.End => End;
    }
}

