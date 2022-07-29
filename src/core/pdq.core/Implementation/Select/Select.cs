using System;
using System.Linq;
using System.Linq.Expressions;
using pdq.common;
using pdq.state;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("pdq.core-tests")]
namespace pdq.Implementation
{
	internal class Select
        : SelectBase, ISelectWithAlias, ISelectFrom, IOrderByThen, IGroupBy, IGroupByThen
	{
        private Select(
            ISelectQueryContext context,
            IQuery query)
            : base(context, (IQueryInternal)query) { }

        public static Select Create(
            ISelectQueryContext context,
            IQuery query)
            => new Select(context, query);

        /// <inheritdoc/>
        internal ISelectQueryContext GetContext() => this.context;

        /// <inheritdoc/>
        public string Alias { get; private set; }

        /// <inheritdoc/>
        public ISelectFrom Join(
            IQueryTarget from,
            state.IWhere conditions,
            IQueryTarget to,
            JoinType type = JoinType.Default)
        {
            this.context.Join(state.Join.Create(from, to, type, conditions));
            return this;
        }

        /// <inheritdoc/>
        public ISelectFrom Join(
            IQueryTarget from,
            state.IWhere conditions,
            Action<ISelectWithAlias> query,
            JoinType type = JoinType.Default)
        {
            var select = Create(this.context, this.query);
            query(select);
            var to = state.QueryTargets.SelectQueryTarget.Create(select.context, select.Alias);

            this.context.Join(state.Join.Create(from, to, type, conditions));
            return this;
        }

        /// <inheritdoc/>
        public IGroupBy Where(Action<IWhereBuilder> builder)
        {
            var b = WhereBuilder.Create(this.options, this.context);
            builder(b);
            this.context.Where(b.GetClauses().First());
            return this;
        }

        /// <inheritdoc/>
        public ISelectFrom From(
            string table,
            string alias,
            string schema = null)
        {
            var managedTable = this.query.AliasManager.GetAssociation(alias) ?? table;
            var managedAlias = this.query.AliasManager.Add(alias, managedTable);
            this.context.From(state.QueryTargets.TableTarget.Create(managedTable, managedAlias, schema));
            return this;
        }

        /// <inheritdoc/>
        public ISelectFrom From(Action<ISelect> query, string alias)
        {
            var select = Create(this.context, this.query);
            query(select);

            this.query.AliasManager.Add(null, alias);
            var builtQuery = state.QueryTargets.SelectQueryTarget.Create(select.context, alias);
            this.context.From(builtQuery);

            return this;
        }

        /// <inheritdoc/>
        public ISelectFromTyped<T> From<T>()
        {
            var table = this.context.Helpers().GetTableName<T>();
            var alias = this.query.AliasManager.Add(null, table);
            this.context.From(state.QueryTargets.TableTarget.Create(table, alias));
            return Select<T>.Create(this.context, this.query);
        }

        /// <inheritdoc/>
        public ISelectFromTyped<T> From<T>(Expression<Func<T, T>> aliasExpression)
        {
            var table = this.context.Helpers().GetTableName(aliasExpression);
            var alias = this.context.Helpers().GetTableAlias(aliasExpression);

            var managedTable = this.query.AliasManager.GetAssociation(alias) ?? table;
            var managedAlias = this.query.AliasManager.Add(alias, table);

            this.context.From(state.QueryTargets.TableTarget.Create(managedTable, managedAlias));
            return Select<T>.Create(this.context, this.query);
        }

        /// <inheritdoc/>
        public ISelectFromTyped<T> From<T>(Action<ISelectWithAlias> query, string alias)
        {
            var select = Create(this.context, this.query);
            query(select);

            var target = state.QueryTargets.SelectQueryTarget.Create(select.context, select.Alias);

            var table = this.context.Helpers().GetTableName<T>();
            this.query.AliasManager.Add(table, alias);

            this.context.From(target);
            return Select<T>.Create(this.context, this.query);
        }

        /// <inheritdoc/>
        public ISelectWithAlias KnownAs(string alias)
        {
            Alias = alias;
            return this;
        }

        /// <inheritdoc/>
        public IOrderByThen OrderBy(string column, string tableAlias, SortOrder orderBy)
        {
            var managedTable = this.query.AliasManager.GetAssociation(tableAlias);
            var managedAlias = this.query.AliasManager.Add(tableAlias, managedTable);
            this.context.OrderBy(state.OrderBy.Create(column, state.QueryTargets.TableTarget.Create(managedTable, managedAlias), orderBy));
            return this;
        }

        /// <inheritdoc/>
        public IGroupByThen GroupBy(string column, string tableAlias)
        {
            var managedTable = this.query.AliasManager.GetAssociation(tableAlias);
            var managedAlias = this.query.AliasManager.Add(tableAlias, managedTable);
            this.context.GroupBy(state.GroupBy.Create(column, state.QueryTargets.TableTarget.Create(managedTable, managedAlias)));
            return this;
        }

        /// <inheritdoc/>
        public IOrderByThen ThenBy(string column, string tableAlias, SortOrder orderBy)
            => OrderBy(column, tableAlias, orderBy);

        /// <inheritdoc/>
        public IGroupByThen ThenBy(string column, string tableAlias)
            => GroupBy(column, tableAlias);

        /// <inheritdoc/>
        IExecuteDynamic ISelectColumn.Select(Expression<Func<ISelectColumnBuilder, dynamic>> expression)
        {
            base.AddColumns(expression);
            return ExecuteDynamic.Create(this.query);
        }

        /// <inheritdoc/>
        IExecute<TResult> ISelectColumn.Select<TResult>(Expression<Func<ISelectColumnBuilder, TResult>> expression)
        {
            base.AddColumns(expression);
            return Execute<TResult>.Create(this.query);
        }
    }
}

