using System;

namespace pdq
{
	public interface ISelectFrom :
		ISelectColumn,
		IJoin
	{
		IGroupBy Where(Action<IWhereBuilder> builder);
	}
}