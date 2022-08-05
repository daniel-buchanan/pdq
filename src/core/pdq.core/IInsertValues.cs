﻿using System;
using System.Collections.Generic;
using pdq.common;

namespace pdq
{
    public interface IInsertValues : IExecute
    {
        IInsertValues Value(dynamic value);
        IInsertValues Values(IEnumerable<dynamic> values);
        IInsertValues From(Action<ISelect> query);
    }
}
