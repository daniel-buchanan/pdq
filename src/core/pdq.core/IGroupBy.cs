using pdq.common;

namespace pdq
{
	public interface IGroupBy : IOrderBy
	{
		IGroupByThen GroupBy(string column, string tableAlias);
	}

	public interface IGroupByThen : IOrderBy
    {
		IGroupByThen ThenBy(string column, string tableAlias);
	}
}

