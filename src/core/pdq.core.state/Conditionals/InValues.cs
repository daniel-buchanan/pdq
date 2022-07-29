using System;
using System.Collections.Generic;
using System.Linq;

namespace pdq.state.Conditionals
{
	public abstract class Values : Where
	{
		public abstract Type ValueType { get; protected set; }

        public static InValues<T> In<T>(state.Column column, IEnumerable<T> values)
        {
            return new InValues<T>(column, values);
        }
	}

    public class InValues<T> : Values, IInValues
    {
        private readonly List<T> values;

        internal InValues(state.Column column, IEnumerable<T> values)
        {
            ValueType = typeof(T);
            Column = column;
            this.values = values?.ToList() ?? new List<T>();
        }

        public state.Column Column { get; private set; }

        public IReadOnlyCollection<T> ValueSet => this.values.AsReadOnly();

        IReadOnlyCollection<object> IInValues.GetValues()
        {
            var result = new List<object>();
            foreach (var o in this.values)
                result.Add(o);
            return result.AsReadOnly();
        }


        public override Type ValueType { get; protected set; }
    }
}

