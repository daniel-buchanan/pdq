using System;
using System.Collections.Generic;

namespace pdq
{
    public interface IColumnValueBuilder
    {
        void EqualTo<T>(T value);

        void Like<T>(T value);
        void StartsWith<T>(T value);
        void EndsWith<T>(T value);

        void In<T>(params T[] values);
        void In<T>(IEnumerable<T> values);

        void LessThan<T>(T value);
        void LessThanOrEqualTo<T>(T value);
        void GreaterThan<T>(T value);
        void GreaterThanOrEqualTo<T>(T value);

        void Between<T>(T start, T end);
    }
}

