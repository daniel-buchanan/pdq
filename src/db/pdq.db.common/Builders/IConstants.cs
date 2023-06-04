﻿using System;
namespace pdq.db.common.Builders
{
	public interface IConstants
	{
        string OpeningParen { get; }
        string ClosingParen { get; }
        string And { get; }
        string Or { get; }
        string Not { get; }
        string Like { get; }
        string OrderBy { get; }
        string GroupBy { get; }
        string ValueQuote { get; }
        string ColumnQuote { get; }
        string Seperator { get; }
        string Ascending { get; }
        string Descending { get; }
        string Comment { get; }
        string Join { get; }
        string On { get; }
        string From { get; }
        string Where { get; }
        string Returning { get; }
        string Values { get; }
        string Limit { get; }
    }
}

