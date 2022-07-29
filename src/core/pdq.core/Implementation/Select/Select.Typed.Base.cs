using System;
using System.Linq.Expressions;
using pdq.common;
using pdq.state;
using pdq.state.QueryTargets;

namespace pdq.Implementation
{
    internal class SelectTypedBase : SelectBase
	{
        protected SelectTypedBase(
            ISelectQueryContext context,
            IQuery query)
            : base(context, (IQueryInternal)query)
        {
        }

        protected void AddJoin<T1, T2>(Expression expression, JoinType type)
        {
            var conditions = this.context.Helpers().ParseJoin(expression);
            var left = GetQueryTarget<T1>();
            var right = GetQueryTarget<T2>();

            this.context.Join(state.Join.Create(left, right, type, conditions));
        }

        protected void AddJoin<T1, T2>(Action<ISelectWithAlias> query, Expression expression, JoinType type)
        {
            var select = Select.Create(this.context, this.query);
            query(select);

            var table = this.context.Helpers().GetTableName<T2>();
            var alias = this.query.AliasManager.Add(select.Alias, table);

            var left = GetQueryTarget<T1>();
            var right = SelectQueryTarget.Create(select.GetContext(), alias);

            var conditions = this.context.Helpers().ParseJoin(expression);

            this.context.Join(state.Join.Create(left, right, type, conditions));
        }

        protected void AddWhere(Expression expression)
        {
            var clause = this.context.Helpers().ParseWhere(expression);
            this.context.Where(clause);
        }

        protected void AddGroupBy(Expression expression)
        {
            var column = this.context.Helpers().GetColumnName(expression);
            var target = GetQueryTarget(expression);
            this.context.GroupBy(state.GroupBy.Create(column, target));
        }

        protected void AddOrderBy(Expression expression, SortOrder order)
        {
            var column = this.context.Helpers().GetColumnName(expression);
            var target = GetQueryTarget(expression);
            this.context.OrderBy(state.OrderBy.Create(column, target, order));
        }

        protected IQueryTarget GetQueryTarget<T>()
        {
            var table = this.context.Helpers().GetTableName<T>();
            return GetQueryTarget(table);
        }
    }
}

