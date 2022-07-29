using System;
using System.Collections.Generic;

namespace pdq.state.Conditionals
{
    public interface IInValues
    {
        state.Column Column { get; }
        IReadOnlyCollection<object> GetValues();
        Type ValueType { get; }
    }
}