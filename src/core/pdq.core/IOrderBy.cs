using pdq.common;

namespace pdq
{
	public interface IOrderBy :
		IExecute,
		ISelectColumn
	{
		IOrderByThen OrderBy(string column, string tableAlias, SortOrder orderBy);
	}

	public interface IOrderByThen :
		IExecute,
		ISelectColumn
	{
		IOrderByThen ThenBy(string column, string tableAlias, SortOrder orderBy);
	}
}

